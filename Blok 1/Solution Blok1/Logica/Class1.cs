using Datalaag;
using Globals;

namespace Logica {
    public class BfInterpreter {
        private FileLoader fileLoader;
        private byte[] memory;
        private Int16 memoryPointer;
        /// <see cref="https://www.tutorialsteacher.com/csharp/csharp-stack"></see>
        private Stack<int> loopPointer;
        private int inputPointer;
        private Commands[] program;
        private char[] preparedInput;
        private Func<char> inputFunction;
        private Action<string> outputFunction;

        public byte[] Memory { get; private set; }
        public Int16 MemoryPointer { get; private set; }
        public string Program { get; set; }


        /// <summary>
        /// setup interne parameters voor programma
        /// </summary>
        /// <param name="inputFunction">funcie die wordt uitgeroepen bij "," commando</param>
        /// <param name="outputFunction">funcie die wordt uitgeroepen bij "." commando</param>
        public BfInterpreter(Func<char> inputFunction, Action<string> outputFunction) {
            this.fileLoader = new FileLoader();
            this.memory = new byte[Int16.MaxValue];
            this.memoryPointer = 0;
            this.loopPointer = new Stack<int>();
            this.inputPointer = 0;

            this.inputFunction = inputFunction;
            this.outputFunction = outputFunction;
            this.program = new Commands[0];
            this.preparedInput = new char[0];
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
            catch (FileNotFoundException e){
                this.outputFunction("file " + programinput + " could not be found");
            }
            catch (Exception e) {
                loadedProgram = programinput;
            }
            this.program = BrainfuckPrecompiler.SimpleEncoding(loadedProgram);
        }

        /// <summary>
        /// zet een predefined input voor het uit te voeren programma, als te weinig input is gegeven wordt deze later gevraagd
        /// </summary>
        /// <param name="input">meegegeven programma in stringvorm of file path</param>
        public void PrepareInput(string input) {
            this.preparedInput = new char[input.Length];
            foreach (char c in input) {
                this.preparedInput[inputPointer] = c;
                inputPointer++;
            }
            this.inputPointer = 0;
        }

        /// <summary>
        /// het uitvoeren van het brainfuck programma gebeurt hier
        /// </summary>
        /// <see cref="https://www.w3schools.com/cs/cs_switch.php"/>
        /// <see cref="https://www.c-sharpcorner.com/UploadFile/mahesh/convert-char-to-byte-in-C-Sharp/"></see>
        public void Interpret() {
            for (int i = 0; i < this.program.Length; i++) {
                
                if(memoryPointer<0 || memoryPointer>this.memory.Length) throw new IndexOutOfRangeException("tried to access index " + memoryPointer.ToString() + " of memory with size " + memory.Length.ToString());
                switch (cmd) {
                    case Commands.Inc:
                        this.memory[memoryPointer]++;
                        break;
                    case Commands.Dec:
                        this.memory[memoryPointer]--;
                        break;
                    case Commands.Right:
                        this.memoryPointer++;
                        break;
                    case Commands.Left:
                        this.memoryPointer--;
                        break;
                    case Commands.Loop:
                        int bracketCount = 0;
                        if (this.memory[memoryPointer] != 0) {
                            this.loopPointer.Push(i);
                        }
                        else {
                            //find index of matching ], dit codeblok hoort namelijk niet te worden uitgevoerd
                            for (int j = i; j < this.program.Length; j++) {
                                if (this.program[i].ToString() == EnumExtender.GetDescriptionOf(Commands.Loop)) bracketCount++;
                                if (this.program[i].ToString() == EnumExtender.GetDescriptionOf(Commands.Jmp)) bracketCount--;

                            }
                        }
                        break;
                    case Commands.Jmp:
                        if (memory[memoryPointer] != 0) i = this.loopPointer.Pop() - 1;
                        break;
                    case Commands.Read:
                        if (inputPointer < this.preparedInput.Length) {
                            this.memory[memoryPointer] = Convert.ToByte(this.preparedInput[inputPointer]);
                        }
                        else {
                            this.memory[memoryPointer] = Convert.ToByte(inputFunction());
                        }
                        break;
                    case Commands.Write:
                        string outp = ((char)this.memory[memoryPointer]).ToString();
                        outputFunction(outp);
                        break;
                }
            }
        }
    }
}
