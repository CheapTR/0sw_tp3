using Godot;
using System;

public class player : KinematicBody2D
{
	Vector2 UP = new Vector2(0, -1);
	const int GRAVITY = 20;
	const int MAXFALLSPEED = 200;
	const int MAXSPEED = 100;
	const int JUMPFORCE = 300;

	const int ACCEL = 10;
	Vector2 vZero = new Vector2();

	bool facing_right = true;


	Vector2 motion = new Vector2();

	Sprite currentSprite;
	AnimationPlayer animPlayer;
		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentSprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(float delta)
	{
		motion.y += GRAVITY;

		if(motion.y > MAXFALLSPEED) {
			motion.y = MAXFALLSPEED;
		}

		if (facing_right) {
			currentSprite.FlipH = false;
		} else {
			currentSprite.FlipH = true;
		}

		 motion.x = motion.Clamped(MAXSPEED).x;

		if (Input.IsActionPressed("ui_left")) {
			motion.x -= ACCEL;
			facing_right = false;
			animPlayer.Play("Run");
		} else if (Input.IsActionPressed("ui_right")) {
			motion.x += ACCEL;
			facing_right = true;
			animPlayer.Play("Run");
		} else {
			motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
			animPlayer.Play("Idle");
		}
		
		if (IsOnFloor())
		{
			if (Input.IsActionPressed("j")) {
			animPlayer.Play("Attack");
			}
		}

		if (IsOnFloor())
			// On ne regarde qu'un seul fois et non le maintient de la touche
			if (Input.IsActionJustPressed("ui_jump")) {
				motion.y = -JUMPFORCE;
				GD.Print($"motion.y = {motion.y}");
				Console.WriteLine($"motion.y = {motion.y}");
			}

		if (!IsOnFloor()) {
			if (motion.y < 0) {
				animPlayer.Play("jump");
			} else if (motion.y > 0) {
				animPlayer.Play("fall");
			}
		}

		motion = MoveAndSlide(motion, UP);
	}


	
}
