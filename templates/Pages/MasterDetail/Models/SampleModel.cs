using Windows.UI.Xaml.Controls;

namespace ItemNamespace.Models
{
    public class SampleModel
    {
        public string Title { get; set; }
        public Symbol Symbol { get; set; }
        public char SymbolAsChar => (char)Symbol;
        public string Description { get; set; }
    }
}