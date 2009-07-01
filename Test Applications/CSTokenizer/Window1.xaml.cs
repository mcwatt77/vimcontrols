using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Linq;
using CSTokenizer.Handlers;
using KeyStringParser;
using Model;

namespace CSTokenizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
//            MessageBox.Show("Step 1. List all of the comments by themselves, with a check box for deleting");

            try
            {
                List<string> temp;
                const string fileName = @"D:\Classapps\src\ResultsOverview.aspx.cs";
//                const string fileName = @"..\..\Window1.xaml.cs";
                var fileInfo = new FileInfo(fileName);
                var codeToParse = fileInfo.OpenText().ReadToEnd();

                var handler = new Handler();
                //TODO: I can make this part of the IStackStream/StackStreamer interface
                //where in I input a stream of character and get out a stream of tokens
                codeToParse.Do(handler.Process);
                handler.Flush();

                var tokens = StackStreamer.Stream(handler);
                tokens = TokenNester.Process(tokens);

                var codeListing = new CodeListing {Assembly = new Assembly()};
                new TreeProcessor(tokens, codeListing).ProcessObjectTree();

                var doc = new XDocument(codeListing.GetXml());
                doc.Save(@"..\..\output.xml");

                MessageBox.Show("Complete!");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\r\n\r\n" + exception.StackTrace);
            }

            Close();
        }
    }
}