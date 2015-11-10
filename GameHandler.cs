using ProjectBueno.Game.Entities;
using System;

namespace ProjectBueno.Engine
{
	class GameHandler : IHandler
    {
        public GameHandler(int xSize, int ySize)
        {
            this.xSize = xSize;
            this.ySize = ySize;
            collisionMap = new bool[xSize,ySize];
        }

        public bool[,] collisionMap;
        public int xSize;
        public int ySize;

        public Player player { get; private set; }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}