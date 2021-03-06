﻿using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Resin
{
    public class FieldWriter : IDisposable
    {
        private readonly string _fileName;
        private readonly IDictionary<string, IDictionary<int, IList<int>>> _terms;

        public FieldWriter(string fileName)
        {
            _fileName = fileName;
            _terms = new Dictionary<string, IDictionary<int, IList<int>>>();
            var dir = Path.GetDirectoryName(_fileName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public void Write(int docId, string token, int position)
        {
            IDictionary<int, IList<int>> docs;
            if (!_terms.TryGetValue(token, out docs))
            {
                docs = new Dictionary<int, IList<int>> {{docId, new List<int> {position}}};
                _terms.Add(token, docs);
            }
            else
            {
                IList<int> positions;
                if (!docs.TryGetValue(docId, out positions))
                {
                    positions = new List<int>();
                    docs.Add(docId, positions);
                }
                positions.Add(position);
            }
        }

        public void Dispose()
        {
            using (var fs = File.Create(_fileName))
            {
                Serializer.Serialize(fs, _terms);
            }
        }
    }
}
