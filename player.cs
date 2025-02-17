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

	AnimationTree animTree;
	
	enum states {NOT_ATTACKING, ATTACKING, AIRBORNE};
	
	states currentState = states.NOT_ATTACKING;

	Vector2 motion = new Vector2();

	Sprite currentSprite;
	
	AnimationPlayer animPlayer;
	AnimationNodeStateMachinePlayback animState;
		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentSprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		
		animTree = GetNode<AnimationTree>("AnimationTree");
		animState = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");
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
		
		switch (currentState)
		{
			case states.NOT_ATTACKING:
				var input_vector = Vector2.Zero;
				if (Input.IsActionPressed("attack")) {
					if (IsOnFloor()) {
						motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
						currentState = states.ATTACKING;
					}
				} else if (Input.IsActionPressed("ui_right")) {
					motion.x += ACCEL;
					facing_right = true;
					animState.Travel("Run");
				} else if (Input.IsActionPressed("ui_left")) {
					motion.x -= ACCEL;
					facing_right = false;
					animState.Travel("Run");
				} else {
					motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
					
					
					animState.Travel("Idle");
				}
				if (IsOnFloor())
					// On ne regarde qu'un seul fois et non le maintient de la touche
					if (Input.IsActionJustPressed("ui_jump")) {
						motion.y = -JUMPFORCE;
						GD.Print($"motion.y = {motion.y}");
						Console.WriteLine($"motion.y = {motion.y}");
						currentState = states.AIRBORNE;
					}
				break;
				
			case states.ATTACKING:
				if((Input.IsActionPressed("attack")))
				{
					motion.x = 0;
					animState.Travel("Attack");
				}
					
				
				break;
			
			case states.AIRBORNE:
				if (!IsOnFloor()) {
					if (motion.y < 0) {
					animState.Travel("Jump");
					} else if (motion.y > 0) {
					animState.Travel("Falling");
					}
					if (Input.IsActionPressed("ui_right")) {
						motion.x += ACCEL;
						facing_right = true;
					} else if (Input.IsActionPressed("ui_left")) {
						motion.x -= ACCEL;
						facing_right = false;
					}
				}
				else currentState = states.NOT_ATTACKING;
			break;
		}

		motion = MoveAndSlide(motion, UP);
	}
	
	public void attack_animation_finished()
	{
		currentState = states.NOT_ATTACKING;
	}


	
}
