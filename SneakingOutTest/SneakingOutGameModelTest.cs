using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using SneakingOut_Xamarin.Model;
using SneakingOut_Xamarin.Persistence;
using System;


namespace SneakingOutTest
{   
    [TestClass]
    public class SneakingOutGameModelTest
    {
        private SneakingOutGameModel _model;
        private SneakingOutTable _mockedTable;
        private Mock<SneakingOutDataAccess> _mock;

        [TestInitialize]
        public void Initialize()
        {
            _mockedTable = new SneakingOutTable();

            for (int i = 0; i < _mockedTable.Size; i++)
            {
                for (int j = 0; j < _mockedTable.Size; j++)
                {
                    _mockedTable.SetValue(i, j, 0); // inicalizalunk egy ures tablat
                }
            }

            // elhelyezunk falakat, oroket, es egy jatekost

            _mockedTable.SetValue(0, 2, 4);
            _mockedTable.SetValue(1, 2, 4); //fal
            _mockedTable.SetValue(2, 2, 4);
            _mockedTable.SetValue(3, 2, 4);
            _mockedTable.SetValue(3, 6, 1); // sec1
            _mockedTable.SetValue(1, 9, 2); // sec2
            _mockedTable.SetValue(9, 8, 5); // exit
            _mockedTable.SetValue(7, 0, 3); // jatekos

            _mock = new Mock<SneakingOutDataAccess>();
            _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                .Returns(() => Task.FromResult(_mockedTable));

            _model = new SneakingOutGameModel(_mock.Object);
            // példányosítjuk a modellt a mock objektummal

            _model.GameAdvanced += new EventHandler<SneakingOutEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<SneakingOutEventArgs>(Model_GameOver);
        }

        [TestMethod]
        public async Task SneakingOutSteptTest()
        {
            await _model.LoadGameAsync(String.Empty);

            _model.PlayerMove(2); // jobbra megy
            Assert.AreEqual(_model.Table.GetValue(7, 1), 3);
            Assert.AreEqual(_model.Table.GetValue(7, 0), 0);
            Assert.AreEqual(_model.GameStepCount, 1);
        }

        [TestMethod]
        public async Task SneakingOutEverythingsMovesTest()
        {
            await _model.LoadGameAsync(String.Empty);

            Assert.AreEqual(_mockedTable.GetValue(3, 6), 1);
            Assert.AreEqual(_mockedTable.GetValue(1, 9), 2);

            //balra mozognak az orok
            _model.SecurityMove(_model.Table._securityOne, 3, 1);
            _model.SecurityMove(_model.Table._securityTwo, 3, 2);

            Assert.AreEqual(_mockedTable.GetValue(3, 5), 1);
            Assert.AreEqual(_mockedTable.GetValue(1, 8), 2);
        }

        [TestMethod]
        public async Task SneakingOutGameModelLoadTest()
        {
            // kezdünk egy új játékot
            _model.NewGame();

            // majd betöltünk egy játékot
            await _model.LoadGameAsync(String.Empty);

            for (Int32 i = 0; i < 10; i++)
                for (Int32 j = 0; j < 10; j++)
                {
                    Assert.AreEqual(_mockedTable.GetValue(i, j), _model.Table.GetValue(i, j));
                    // ellenõrizzük, valamennyi mezõ értéke megfelelõ-e
                }

            // a lépésszám 0-ra áll vissza
            Assert.AreEqual(0, _model.GameStepCount);

            // ellenõrizzük, hogy meghívták-e a Load mûveletet a megadott paraméterrel
            _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
        }

        [TestMethod]
        public void SneakingOutGameModelNewGameTest()
        {
            // kezdünk egy új játékot
            _model.NewGame();

            // a lépésszám 0-ra áll vissza
            Assert.AreEqual(0, _model.GameStepCount);
            Assert.AreEqual(0, _model.GameTime);
            Assert.IsFalse(_model.IsGameOver);
            Assert.IsFalse(_model.Table._isEscaped);


        }

        private void Model_GameAdvanced(object sender, SneakingOutEventArgs e)
        {
            _model.AdvanceTime();
            Assert.IsTrue(_model.GameTime == 1);
            Assert.AreEqual(_model.GameTime > 0, _model.IsGameOver); // a tesztben a játéknak csak akkor lehet vége, ha lejárt az idõ

            Assert.AreEqual(e.GameStepCount, _model.GameStepCount); // a két értéknek egyeznie kell
            Assert.AreEqual(e.GameTime, _model.GameTime); // a két értéknek egyeznie kell
            Assert.IsFalse(e.IsWon); // még nem nyerték meg a játékot
        }

        private void Model_GameOver(object sender, SneakingOutEventArgs e)
        {
            Assert.IsTrue(_model.IsGameOver); // biztosan vége van a játéknak
            Assert.IsFalse(e.IsWon);
        }
    }
}
