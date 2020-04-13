﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonogameTest1
{
	public class Camera
	{
		public float Zoom { get; set; }
		public Vector2 Position { get; set; }
		public Rectangle Bounds { get; protected set; }
		public Rectangle VisibleArea { get; protected set; }
		public Matrix Transform { get; protected set; }

		private float currentMouseWheelValue, previousMouseWheelValue, zoom, previousZoom;

		public Camera(Viewport viewport)
		{
			Bounds = viewport.Bounds;
			Zoom = 1f;
			Position = Vector2.Zero;
		}

		public float MaxZoom = 10f;
		public float MinZoom = 0.1f;

		private void UpdateVisibleArea()
		{
			var inverseViewMatrix = Matrix.Invert(Transform);

			var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
			var tr = Vector2.Transform(new Vector2(Bounds.X, 0), inverseViewMatrix);
			var bl = Vector2.Transform(new Vector2(0, Bounds.Y), inverseViewMatrix);
			var br = Vector2.Transform(new Vector2(Bounds.Width, Bounds.Height), inverseViewMatrix);

			var min = new Vector2(
				MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
				MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));

			var max = new Vector2(
				MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
				MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));

			VisibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
		}

		private void UpdateMatrix()
		{
			Transform
				= Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0))
				* Matrix.CreateScale(Zoom)
				* Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));

			UpdateVisibleArea();
		}

		public void MoveCamera(Vector2 movePosition)
		{
			Position += movePosition;
		}

		public void Follow(Vector2 movePosition)
		{
			Position = movePosition;
		}

		public void AdjustZoom(float zoomAmount)
		{
			Zoom += zoomAmount;
			Zoom = MathHelper.Clamp(Zoom, MinZoom, MaxZoom);
		}

		public void UpdateCamera(Viewport bounds)
		{
			Bounds = bounds.Bounds;
			UpdateMatrix();

			Vector2 cameraMovement = Vector2.Zero;
			var moveSpeed = (int)(-20 * Zoom + 35);

			//if (Keyboard.GetState().IsKeyDown(Keys.Up))
			//{
			//	cameraMovement.Y = -moveSpeed;
			//}

			//if (Keyboard.GetState().IsKeyDown(Keys.Down))
			//{
			//	cameraMovement.Y = moveSpeed;
			//}

			//if (Keyboard.GetState().IsKeyDown(Keys.Left))
			//{
			//	cameraMovement.X = -moveSpeed;
			//}

			//if (Keyboard.GetState().IsKeyDown(Keys.Right))
			//{
			//	cameraMovement.X = moveSpeed;
			//}

			previousMouseWheelValue = currentMouseWheelValue;
			currentMouseWheelValue = Mouse.GetState().ScrollWheelValue;

			if (currentMouseWheelValue > previousMouseWheelValue)
			{
				AdjustZoom(0.2f * Zoom);
			}

			if (currentMouseWheelValue < previousMouseWheelValue)
			{
				AdjustZoom(0.2f * -Zoom);
			}

			previousZoom = zoom;
			zoom = Zoom;

			MoveCamera(cameraMovement);
		}
	}
}