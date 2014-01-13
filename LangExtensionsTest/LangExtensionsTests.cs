using System;
using System.Globalization;
using System.Text.Lang;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LangExtensionsTest
{
    [TestClass]
    public class LangExtensionsUnitTests
    {
        [TestMethod]
        public void SoundsLikeIsTrueTest()
        {
            const string engText = "Hello";
            Assert.IsTrue(engText.SoundsLike(CultureInfo.GetCultureInfo("en-us")).GetValueOrDefault());
        }
        [TestMethod]
        public void SoundsLikeIsFalseTest()
        {
            const string engText = "Ghbdtn";
            Assert.IsFalse(engText.SoundsLike(CultureInfo.GetCultureInfo("en-us")).GetValueOrDefault());
        }

        [TestMethod]
        public void SoundsLikeCantSureTest()
        {
            const string engText = "Gh";
            Assert.IsFalse(engText.SoundsLike(CultureInfo.GetCultureInfo("en-us")).HasValue);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SoundsLikeShouldThrowExceptionTest()
        {
            const string engText = "Hello";
            engText.SoundsLike(CultureInfo.GetCultureInfo("ru-ru"));
        }

        private static void CheckTransliterate(string source, string expected)
        {
            string result;
            var actual = source.TryMapLayout(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("ru-ru"), out result);
            Assert.IsTrue(actual);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TryTransiterateTest()
        {
            const string cyrText = "f,dult`;pbqrkvyjghcnea[wxioms]'.z~{}:\"<>";
            const string expected = "абвгдеёжзийклмнопрстуфхцчшщьыъэюяЁХЪЖЭБЮ";

            //var upperCyrText = cyrText.ToUpper();
            //var upperExpectedText = expected.ToUpper();

            CheckTransliterate(cyrText, expected);
            //CheckTransliterate(upperCyrText, upperExpectedText);                                               
        }

        [TestMethod]
        public void TryTransiterateIsFalseTest()
        {
            const string cyrText = "Vf34";
            string result;
            var actual = cyrText.TryMapLayout(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("ru-ru"), out result);
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void CorrectLayoutTest()
        {
            const string cyrText = "Ghbdtn";
            string result;
            var actual = cyrText.CorrectLayout(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("ru-ru"),out result);
            Assert.IsTrue(actual);
            Assert.AreEqual("Привет", result);
        }
        [TestMethod]
        public void CorrectLayoutIsFalseTest()
        {
            const string cyrText = "Hello";
            string result;
            var actual = cyrText.CorrectLayout(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("ru-ru"), out result);
            Assert.IsFalse(actual);
            Assert.AreEqual(string.Empty, result);
        }
        [TestMethod]
        public void CorrectLayoutTooShortTest()
        {
            const string cyrText = "Hel";
            string result;
            var actual = cyrText.CorrectLayout(CultureInfo.GetCultureInfo("en-us"), CultureInfo.GetCultureInfo("ru-ru"), out result);
            Assert.IsFalse(actual);
            Assert.AreEqual(string.Empty, result);
        }
        [TestMethod]
        public void CorrectLayoutAsyncTest()
        {
            const string cyrText = "Ghbdtn";
            var actual = cyrText.CorrectLayoutAsync(CultureInfo.GetCultureInfo("en-us"),CultureInfo.GetCultureInfo("ru-ru")).Result;
            Assert.IsTrue(actual.IsSucceeded);
            Assert.AreEqual(actual.Text, "Привет");
 
        }
    }
}
