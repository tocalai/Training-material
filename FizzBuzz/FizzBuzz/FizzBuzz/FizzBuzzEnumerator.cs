using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzz
{
    public class FizzBuzzEnumerator : IEnumerator<string>
    {
        private int _current = 0;
        private int _start;
        private int _end;
        private string _output = string.Empty;

        public string Current => this._output;

        public int CurrentIndex => this._current;

        object IEnumerator.Current => this._current;

        string IEnumerator<string>.Current => this._output;

        public FizzBuzzEnumerator(int start, int end)
        {
            Debug.Assert(end > start);
            this._start = start;
            this._end = end;
            this._current = start - 1;
            this._output = string.Empty;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual bool MoveNext()
        {
            _current++;
            if (this._current % 3 == 0 && this._current % 5 == 0)
            {
                this._output = "FizzBuzz";
            }
            else if (this._current % 5 == 0)
            {
                this._output = "Buzz";
            }
            else if (this._current % 3 == 0)
            {
                this._output = "Fizz";
            }
            else
            {
                this._output = this._current.ToString();
            }
 
            return !(_current > _end);
        }

        public void Reset()
        {
            this._current = 0;
            this._output = string.Empty;
        }
    }
}
