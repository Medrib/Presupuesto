namespace TestPresupuesto
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            bool res = Presupuesto.Repository.ValidacionUsuarioDB.ResponseValidarTrue();
            Assert.AreEqual(true , res);
        }

        [TestMethod]
        public void TestUser()
        {
            bool res = Presupuesto.Repository.ValidacionUsuarioDB.ResponseValidarFalse();
            Assert.AreEqual(false , res);
        }
        [TestMethod]
        public void TestUser2()
        {
            bool res = Presupuesto.Repository.ValidacionUsuarioDB.TestValidarUsuario("pepito", "1234");
            Assert.AreEqual(true , res);
            
        }
    }
}