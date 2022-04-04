using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;

namespace Serialize
{
    class Program
    {
        static void Main(string[] args)
        {
            string serializationType = GetSerializationType();
            string serializedInputObject = GetInputObject();

            if (string.IsNullOrEmpty(serializationType))
            {
                System.Environment.Exit(0);
            }

            try
            {
                Input inpt = DeserializeInputObject(serializationType, serializedInputObject);

                if (inpt is null)
                {
                    Console.WriteLine("Input object is empty");
                    System.Environment.Exit(0);
                }

                Output outp = GetOutputObject(inpt);

                string serializedOutp = SerializeOutput(outp, serializationType);

                Console.WriteLine(serializedOutp);
            } catch (Exception e)
            {
                Console.Write($"Something went wrong: {e.Message}");
            }
        }

        private static string GetSerializationType()
        {
            string type = Console.ReadLine().Trim().ToLower();

            if (type != "json" && type != "xml")
            {
                Console.WriteLine($"Wrong type of serialization: {type}");
                type = string.Empty;
            }

            return type;
        }

        private static string GetInputObject() => Console.ReadLine();

        private static Input DeserializeInputObject(string type, string inputString)
        {
            return type switch
            {
                "json" => DeserializeJson(inputString),
                "xml" => DeserializeXml(inputString),
                _ => null,
            };
        }

        private static Output GetOutputObject(Input inpt)
        {
            Output outp = new();
            outp.SumResult = inpt.K * inpt.Sums.Sum();
            outp.MulResul = inpt.Muls.Aggregate((currentProduct, nextFactor) => currentProduct * nextFactor);

            // merge arrays
            var firstArray = inpt.Muls.Select(x => (decimal)x).ToArray();
            int firstArrayLength = firstArray.Length;
            Array.Resize(ref firstArray, firstArrayLength + inpt.Sums.Length);
            Array.Copy(inpt.Sums, 0, firstArray, firstArrayLength, inpt.Sums.Length);
            Array.Sort(firstArray);
            outp.SortedInputs = firstArray;

            return outp;
        }

        private static string SerializeOutput(Output outp, string type)
        {
            return type switch
            {
                "json" => SerializeJson(outp),
                "xml" => SerializeXml(outp),
                _ => string.Empty,
            };
        }

        private static string SerializeXml(Output outp)
        {
            XmlSerializer xmlFormatter = new(typeof(Output));

            Stream ms = new MemoryStream();
            
            var xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings { Indent = false });
            xmlFormatter.Serialize(xmlWriter, outp);

            ms.Position = 0;
            StreamReader reader = new(ms);
            return reader.ReadToEnd();
        }

        private static Input DeserializeXml(string input)
        {
            XmlSerializer xmlFormatter = new(typeof(Input));

            Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input));

            return (Input)xmlFormatter.Deserialize(ms);
        }

        private static string SerializeJson(Output outp) => JsonSerializer.Serialize(outp);

        private static Input DeserializeJson(string input) => JsonSerializer.Deserialize<Input>(input);
    }
}
