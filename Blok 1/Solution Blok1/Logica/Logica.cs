using Datalaag;
using Globals;

namespace Logica {
    public class BfInterpreter {
        private FileLoader fileLoader;
        /// <see cref="https://www.tutorialsteacher.com/csharp/csharp-stack"></see>
        private Stack<int> loopPointer;
        private int inputPointer;
        private Programdata program;
        private List<byte> memory;

        /// <see cref="https://learn.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.sdk.sfc.readonlylist-1?view=sql-smo-160"</see>
        public IReadOnlyList<byte> MemoryView { get; }
        public Int16 MemoryPointer { get; private set; }
        public Programdata Program { get; private set; }
        public string PreparedInput { get; set; }
        public Func<char> InputFunction { private get;  set; }
        public Action<string> OutputFunction { private get;  set; }
        public Action Tick { private get; set; }


        /// <summary>
        /// setup interne parameters voor programma
        /// </summary>
        /// <param name="inputFunction">funcie die wordt uitgeroepen bij "," commando</param>
        /// <param name="outputFunction">funcie die wordt uitgeroepen bij "." commando</param>
        public BfInterpreter(Func<char> inputFunction, Action<string> outputFunction) {
            this.fileLoader = new FileLoader();
            this.MemoryView = memory.AsReadOnly();
            this.MemoryPointer = 0;
            this.loopPointer = new Stack<int>();
            this.inputPointer = 0;

            this.InputFunction = inputFunction;
            this.OutputFunction = outputFunction;
            this.Program = new Programdata("");
            this.PreparedInput = "";
            this.Tick = () => {};
        }

        /// <summary>
        /// laad een brainfuck programma van string of file
        /// </summary>
        /// <param name="programinput"></param>
        /// <see cref="https://stackoverflow.com/questions/3137097/check-if-a-string-is-a-valid-windows-directory-folder-path"></see>
        public void LoadProgram(string programinput) {
            string loadedProgram="";
            try {
                Path.GetFullPath(programinput);
                loadedProgram = this.fileLoader.Load(programinput);
            }
            catch (FileNotFoundException){
                this.OutputFunction("file " + programinput + " could not be found");
            }
            catch (Exception) {
                loadedProgram = programinput;
            }
            this.Program = new Programdata(loadedProgram);
        }

        /// <summary>
        /// het uitvoeren van het brainfuck programma gebeurt hier
        /// </summary>
        /// <see cref="https://www.w3schools.com/cs/cs_switch.php"/>
        /// <see cref="https://www.c-sharpcorner.com/UploadFile/mahesh/convert-char-to-byte-in-C-Sharp/"></see>
        public void Interpret() {
            for (int i = 0; i < this.Program.Length; i++) {
                Commands cmd = Program.Compiled[i];
                //Console.WriteLine(cmd);
                if(MemoryPointer<0 || MemoryPointer>this.MemoryView.Count) throw new IndexOutOfRangeException("tried to access index " + MemoryPointer.ToString() + " of memory with size " + this.MemoryView.Count.ToString());
                switch (cmd) {
                    case Commands.Inc:
                        this.memory[MemoryPointer]++;
                        break;
                    case Commands.Dec:
                        this.memory[MemoryPointer]--;
                        break;
                    case Commands.Right:
                        this.MemoryPointer++;
                        break;
                    case Commands.Left:
                        this.MemoryPointer--;
                        break;
                    case Commands.Loop:
                        int bracketCount = 0;
                        if (this.MemoryView[MemoryPointer] != 0) {
                            this.loopPointer.Push(i);
                        }
                        else {
                            //find index of matching ], dit codeblok hoort namelijk niet te worden uitgevoerd
                            for (int j = i; j < this.Program.Length; j++) {
                                if (this.Program.Compiled[i] == Commands.Loop) bracketCount++;
                                if (this.Program.Compiled[i]== Commands.Jmp) bracketCount--;
                            }
                        }
                        break;
                    case Commands.Jmp:
                        if (this.MemoryView[MemoryPointer] != 0) i = this.loopPointer.Pop() - 1;
                        break;
                    case Commands.Read:
                        if (inputPointer < this.PreparedInput.Length) {
                            this.MemoryView.ElementAt(MemoryPointer) = Convert.ToByte(this.PreparedInput[inputPointer]);
                        }
                        else {
                            this.MemoryView.ElementAt(MemoryPointer) = Convert.ToByte(InputFunction());
                        }
                        break;
                    case Commands.Write:
                        string outp = ((char)this.MemoryView.ElementAt(MemoryPointer)).ToString();
                        OutputFunction(outp);
                        break;
                }
            }
        }
    }
}
