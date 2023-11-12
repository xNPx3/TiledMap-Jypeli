using System;
using Android.Content.Res;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

public class Tasohyppelypeli : PhysicsGame
{
    PlatformCharacter player;
    TiledMap map;
    double moveSpeed = 100;

    public override void Begin()
    {
        Level.BackgroundColor = Color.LightBlue;
        //Game.FixedTimeStep = false;
        //Game.UpdatesPerSecod = 30;
        Gravity = new Vector(0, -1500);

        map = new TiledMap("map.tmj", "tileset.tsj");
        map.SetOverride(108, CreateJumpBlock);
        map.SetOverride(178, CreatePlayer);
        map.SetOverride(67, CreateDiamond);
        map.SetOverride(68, CreateSpikes);
        map.Execute();
    }

    private void CreateDiamond(Vector position, double width, double height, Image tileImage)
    {
        PhysicsObject diamond = PhysicsObject.CreateStaticObject(width, height);
        diamond.Position = position;
        diamond.Image = tileImage;
        diamond.Tag = "diamond";
        diamond.Shape = Shape.Circle;
        diamond.Oscillate(Vector.UnitY, 4, 0.8);
        Add(diamond, 2);
    }

    private void CreateSpikes(Vector position, double width, double height, Image tileImage)
    {
        PhysicsObject spike = PhysicsObject.CreateStaticObject(width, height);
        spike.Position = position;
        spike.Image = tileImage;
        spike.Tag = "spike";
        spike.Shape = Shape.FromImage(tileImage);
        Add(spike, -2);
    }

    private void CreatePlayer(Vector position, double width, double height, Image tileImage)
    {
        player = new PlatformCharacter(16, 16);
        player.Position = position;
        player.CollisionIgnoreGroup = 1;

        Image p = map.GetTileImage(146, map.tilesets[0]);
        p = Image.Mirror(p);
        p.Scaling = ImageScaling.Nearest;
        player.Image = p;

        Add(player, 1);

        Camera.Follow(player);
        Camera.StayInLevel = true;
        Camera.ZoomFactor = 10;

        AddCollisionHandler(player, "jump block", Boost);
        AddCollisionHandler(player, "diamond", CollectDiamond);
        AddCollisionHandler(player, "spike", (p, o) => player.Position = position);

        //TouchPanel.Listen(ButtonState.Pressed, (t) => player.Jump(400), null);
        TouchPanel.Listen(ButtonState.Down, TapHandler, null);
    }

    private void CollectDiamond(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        otherObject.IgnoresCollisionResponse = true;
        otherObject.Destroy();
    }

    private void Boost(IPhysicsObject collidingObject, IPhysicsObject otherObject)
    {
        if (player.Y > otherObject.Y)
        {
            otherObject.Image = map.GetTileImage(108, map.tilesets[0]);
            player.ForceJump(1000);
            Timer.SingleShot(1, () => otherObject.Image = map.GetTileImage(109, map.tilesets[0]));
        }
    }

    private void CreateJumpBlock(Vector position, double width, double height, Image tileImage)
    {
        PhysicsObject jumpBlock = PhysicsObject.CreateStaticObject(width, height);
        jumpBlock.Position = position;
        jumpBlock.Image = tileImage;
        jumpBlock.Tag = "jump block";
        Add(jumpBlock, -2);
    }

    private void TapHandler(Touch touch)
    {
        if (touch.PositionOnScreen.Y < (-Screen.Height / 6))
        {
            if (touch.PositionOnScreen.X > 0)
            {
                player.Walk(moveSpeed);
            }
            else
            {
                player.Walk(-moveSpeed);
            }
        }
        else
        {
            player.Jump(450);
        }
    }

    protected override void Paint(Canvas canvas)
    {
        base.Paint(canvas);
    }

    protected override void Update(Time time)
    {
        base.Update(time);
    }
}
