using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Snake.Persistence;

namespace Snake.Model.Tests {
    [TestClass]
    public class SnakeModelTest {

        private SnakeModel model;               //The model to be tested.
        private PrivateObject wrappedModel;     //Wrapped model for private method testing.
        private Mock<IPersistence> mock;        //Mock object for simulating the persistence.

        [TestInitialize]
        public void Initialize() {
            mock = new Mock<IPersistence>();
            mock.Setup(x => x.LoadLevel(It.IsAny<string>()));
            mock.SetupGet(x => x.MapSize).Returns(7);
            mock.SetupGet(x => x.Walls).Returns(new System.Collections.Generic.List<Tuple<int, int>>() { new Tuple<int, int>(0, 0) });

            model = new SnakeModel(mock.Object);
            wrappedModel = new PrivateObject(model);
        }

        [TestMethod]
        public void SnakeLoadLevelTest() {
            //load the level
            model.LoadLevel(string.Empty);
            //check the map size
            Assert.AreEqual(model.MapSize, 7);
            //check if there is a wall in the correct place
            Assert.AreEqual(model[0, 0], TileType.Wall);
            //check if the snake is in place
            for (int i = 1; i <= 5; i++) {
                Assert.AreEqual(model[3, i],TileType.Snake);
            }
            //check if the snake is oriented right
            Assert.AreEqual(model.Direction, Direction.Right);
            //check if the timer is running
            Assert.AreEqual(model.Paused, false);
            //check if the score is right
            Assert.AreEqual(model.Score, 0);
            //check if load was called
            mock.Verify(mock => mock.LoadLevel(string.Empty), Times.Once());
        }

        [TestMethod]
        public void SnakeRestartLevelTest() {
            model.LoadLevel(string.Empty);
            //restart the level
            model.RestartLevel();
            //check the map size
            Assert.AreEqual(model.MapSize, 7);
            //check if there is a wall in the correct place
            Assert.AreEqual(model[0, 0], TileType.Wall);
            //check if the snake is in place
            for (int i = 1; i <= 5; i++) {
                Assert.AreEqual(model[3, i], TileType.Snake);
            }
            //check if the snake is oriented right
            Assert.AreEqual(model.Direction, Direction.Right);
            //check if the timer is running
            Assert.AreEqual(model.Paused, false);
            //check if the score is right
            Assert.AreEqual(model.Score, 0);
        }

        [TestMethod]
        public void SnakeMoveTest() {
            model.LoadLevel(string.Empty);
            wrappedModel.Invoke("MoveSnake");
            Assert.AreEqual(model[3,6], TileType.Snake);
            model.Direction = Direction.Up;
            wrappedModel.Invoke("MoveSnake");
            Assert.AreEqual(model[2, 6], TileType.Snake);
        }

        [TestMethod]
        public void SnakeSpawnFoodTest() {
            model.LoadLevel(string.Empty);
            wrappedModel.Invoke("SpawnFood");
            bool foundFood = false;
            for (int i = 0; i < model.MapSize; i++) {
                for (int j = 0; j < model.MapSize; j++) {
                    foundFood |= model[i, j] == TileType.Food;
                }
            }
            Assert.IsTrue(foundFood);
        }

        [TestMethod]
        public void SnakeDirectionTest() {
            model.LoadLevel(string.Empty);
            Assert.AreEqual(model.Direction, Direction.Right);
            //the snake should not be able to do a 180
            model.Direction = Direction.Left;
            Assert.AreEqual(model.Direction, Direction.Right);
            model.Direction = Direction.Up;
            Assert.AreEqual(model.Direction, Direction.Up);
        }

        [TestMethod]
        public void SnakeGameOverTest() {
            model.LoadLevel(string.Empty);
            bool eventRaised = false;
            model.GameOver += delegate (object sender, GameOverEventArgs e) {
                eventRaised = true;
            };
            wrappedModel.Invoke("MoveSnake");
            wrappedModel.Invoke("MoveSnake");
            //check if the game over event was raised
            Assert.IsTrue(eventRaised);
        }

    }
}
