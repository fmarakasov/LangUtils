namespace System.Text.Lang
{
    public struct TextResult
    {
        private readonly bool _isSucceeded;
        private readonly string _text;

        public TextResult(bool isSucceeded, string text)
        {
            _isSucceeded = isSucceeded;
            _text = text;
        }

        public bool IsSucceeded
        {
            get { return _isSucceeded; }
        }

        public string Text
        {
            get { return _text; }
        }
    }
}