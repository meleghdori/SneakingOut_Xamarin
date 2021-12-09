using System;
using System.Collections.Generic;
using System.Text;

namespace SneakingOut_Xamarin.Persistence
{
	public class Security
	{
		private Int32 TablePositionX;
		private Int32 TablePositionY;
		private Boolean Up;
		private Boolean Down;
		private Boolean Right;
		private Boolean Left;

		public Security(Int32 tablePositionX, Int32 tablePositionY)
		{
			TablePositionX = tablePositionX;
			TablePositionY = tablePositionY;
			Up = false;
			Down = false;
			Right = false;
			Left = true;
		}

		public Int32 getPositionX()
		{
			return TablePositionX;
		}
		public Int32 getPositionY()
		{
			return TablePositionY;
		}

		public void setPositionX(Int32 tablePositonX)
		{
			TablePositionX = tablePositonX;
		}
		public void setPositionY(Int32 tablePositonY)
		{
			TablePositionY = tablePositonY;
		}

		public Int32 getDirection()
		{
			// 0-up 1-down 2-right 3-left
			if (Up) { return 0; }
			if (Down) { return 1; }
			if (Right) { return 2; }
			if (Left) { return 3; }
			return -1; //?
		}

		public void setDirection(Int32 direction)
		{
			if (direction == 0) { Up = true; Down = false; Right = false; Left = false; }
			if (direction == 1) { Down = true; Up = false; Right = false; Left = false; }
			if (direction == 2) { Right = true; Up = false; Down = false; Left = false; }
			if (direction == 3) { Left = true; Up = false; Down = false; Right = false; }
		}
	}
}
