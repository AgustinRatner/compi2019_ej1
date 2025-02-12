using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
//using at.jku.ssw.cc.tests;
using text_Box_Mio;
//using compilador;

namespace at.jku.ssw.cc
{
    public class ElemPilita
    {
        public enum ElemDePila { esEntero, esEstring };
        public int entero;
        public string estring;
        public ElemDePila elemDePila;
        public ElemPilita(int entero, string estring, ElemDePila elemDePila)
        { this.entero = entero; this.estring = estring; this.elemDePila = elemDePila; }
    }

    public class PilaMia : Pila
    {
        public PilaMia(int cantElem) : base(cantElem) { }

        public void mostrarPilita()
        {
            string pilaString = "";
            for (int i = this.cantMaxDeElem - 1; i >= 0; i--)
            {
                if (i > Parser.pilita.tope)

                    pilaString = pilaString + "\n" + " ";
                else
                {
                    string elemParaMostrar;
                    if (((ElemPilita)elementos[i]).elemDePila == ElemPilita.ElemDePila.esEntero)
                        elemParaMostrar = ((ElemPilita)elementos[i]).entero.ToString();
                    else elemParaMostrar = ((ElemPilita)elementos[i]).estring;
                    pilaString = pilaString + "\n" + elemParaMostrar;

                }
            }
            Program1.form1.richTextBox2.Text = pilaString;
            if (Parser.muestraCargaDeInstrs) Parser.MessageBoxCon2PregMaqVirtual();
        }

        public void inicializa()
        {
            for (int i = this.cantMaxDeElem - 1; i >= 0; i--)
            {
                elementos[i] = new ElemPilita(0, "", ElemPilita.ElemDePila.esEntero);
            }
        }

    }

    class Parser
    {
        public static FrmContinuar mens = new FrmContinuar();
        public static FrmContinuarMaqVirtual mensMaqVirtual = new FrmContinuarMaqVirtual();

        public static bool muestraProducciones = true;
        public static bool muestraCargaDeInstrs = true;
        public static bool ejecuta = false;

        public static PilaMia pilita = new PilaMia(10);

        public static void MessageBoxCon2PregMaqVirtual()
        {
            mensMaqVirtual.ShowDialog();
        }
        public static void MessageBoxCon3Preg(System.Windows.Forms.TreeNode ultimoNodo)
        {
            if (muestraProducciones)
            {
                //Program1.form1.instContinuar.ShowDialog();
                if (ultimoNodo.IsVisible == false) ultimoNodo.EnsureVisible();
                if (ultimoNodo.LastNode != null) ultimoNodo.LastNode.EnsureVisible();
            }
        }

        public static void MessageBoxCon3Preg()
        {
            if (muestraProducciones)
                Program1.form1.instContinuar.ShowDialog();
            Program1.form1.treeView1.ExpandAll();
        }//Fin MessageBoxCon3Preg()


        public enum AccionInstr
        {
            nop, loadLocal, storeLocal, add, sub, mul, div, pop,
            loadConst, write, writeln, branchInc, tJump, fJump, ldstr, ret
        }

        public class Instruccion
        {
            public AccionInstr accionInstr;
            public int nro;
            public int nroLinea;
            public string argDelWriteLine;
            public Code.BrfalseENUM indBrFalse;
            public Code.BrtrueENUM indBrTrue;
            public string instrString;


            public Instruccion(AccionInstr accionInstr, int nro, int nroLinea, string argDelWriteLine,
                               Code.BrtrueENUM indBrTrue, Code.BrfalseENUM indBrFalse, string instrString)
            {
                this.accionInstr = accionInstr; this.nro = nro;
                this.nroLinea = nroLinea; this.argDelWriteLine = argDelWriteLine;
                this.indBrTrue = indBrTrue; this.indBrFalse = indBrFalse; this.instrString = instrString;
            }
        }
        public static bool yaPintada = false;
        public const int maxnroDeInstrCorriente = 200;
        public static int nroDeInstrCorriente;
        public static Instruccion[] cil;
        static string[] namesAccionInstr = { "nop", "ldloc.", "stloc.", "add", "sub", "mul", "div", "pop",
                                         "ldc.i4 ", "write", "writeln", "br", "br_tJump" ,
                                         "br_fJuamp", "ldstr", "ret"};

        public static void inicializaCil()
        {
            cil = new Instruccion[maxnroDeInstrCorriente];
            for (int i = 0; i < maxnroDeInstrCorriente; i++)
                cil[i] = new Instruccion(AccionInstr.nop, 0, 0, "",
                                         Code.BrtrueENUM.BEQenum, Code.BrfalseENUM.BEQenum, "");
        }

        public static void muestraCil()
        {
            string todasLasInstrs = "Accion\tNro\tLinea";
            System.Windows.Forms.MessageBox.Show(maxnroDeInstrCorriente.ToString());

            for (int i = 0; i < maxnroDeInstrCorriente; i++)
            {
                todasLasInstrs = todasLasInstrs + "\n"
                                    + i.ToString() + ":"
                                    + namesAccionInstr[(int)cil[i].accionInstr] + "\t"
                                    + cil[i].nro.ToString() + "\t"
                                    + cil[i].nroLinea.ToString()
                                    + cil[i].argDelWriteLine;
            }
            Program1.form1.richTextBox5.Text = todasLasInstrs;
            System.Windows.Forms.MessageBox.Show("fin");
        }

        public const int maxCantVarsLocales = 10;
        public static int cantVarLocales;
        public static int[] locals = new int[maxCantVarsLocales];

        public static void muestraVarsLocales()
        {
            string todasLasVarsLocales;
            if (cantVarLocales == 0) todasLasVarsLocales = "No hay vars locales";
            else
            {
                todasLasVarsLocales = locals[0].ToString();
                for (int i = 1; i < cantVarLocales; i++)
                {
                    todasLasVarsLocales = todasLasVarsLocales + "\n" + locals[i].ToString();
                }
                Program1.form1.richTextBox4.Text = todasLasVarsLocales;
            }
        }

        static void ParteFinal1()
        {
            Type ptType1 = Code.program.CreateType();
            object ptInstance1 = Activator.CreateInstance(ptType1, new object[0]);
            ptType1.InvokeMember("Main", BindingFlags.InvokeMethod, null, ptInstance1, new object[0]);
            Code.assembly.Save("Piripipi" + ".exe");

            if (ZZ.readKey) Console.ReadKey();
        }//Fin ParteFinal1

        static void ReadKeyMio()
        {
            Code.il.EmitCall(OpCodes.Call, typeof(Console).GetMethod("Read", new Type[0]), null);
            Code.il.Emit(OpCodes.Conv_U2);
            Code.il.Emit(Code.POP);
        }

        public const int
            PROGRAM = 1, CONSTDECL = 2, VARDECL = 3, CLASSDECL = 4, METHODDECL = 5, FORMPARS = 6,
            TYPE = 7, STATEMENT = 8, BLOCK = 9, ACTPARS = 10, CONDITION = 11, CONDTERM = 12,
            CONDFACT = 13, EXPR = 14, TERM = 15, FACTOR = 16, DESIGNATOR = 17, RELOP = 18,
            ADDOP = 19, MULOP = 20, IDENT = 21;

        //static TextWriter output;
        public static Token token;    // last recognized token
        public static Token laToken;  // lookahead token (not yet recognized)
        static int la;         // shortcut to kind attribute of lookahead token (laToken.kind)

        /* Symbol table object of currently compiled method. */
        internal static Symbol curMethod;

        /* Special Label to represent an undefined break destination. */
        //static readonly Label undef;

        /* Reads ahead one symbol. */
        static void Scan()
        {
            token = laToken;
            laToken = Scanner.Next();
            //La 1� vez q se ejecuta, token queda con Token(1, 1), laToken con "class" (primer token del programa)
            la = laToken.kind;
        }

        /* Verifies symbol and reads ahead. */
        static void Check(int expected) //expected viene de la gramatica,  la del laToken que ley�
        {
            if (la == expected)
                Scan();
            else
                Errors.Error("Se esperaba un: " + Token.names[expected]);
        }

        static void Program()
        {
            System.Windows.Forms.TreeNode program = new System.Windows.Forms.TreeNode();  //Crea el nodo program
            program.Text = "Program"; //Texto del nodo "program"
            Program1.form1.treeView1.Nodes.Add(program);//"cuelga" el nodo (raiz) "program" del treeView1 ya creado
            Parser.MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(0);  //pinta de rojo    Program = 'class' ident PosDeclars '{' MethodDeclsOpc '}'.
            Parser.MessageBoxCon3Preg();
            program.Nodes.Add("class");
            program.ExpandAll(); //Visualiza (Expande) hijo de Program
            Parser.MessageBoxCon3Preg();
            //antes del Check (Token.CLASS), token = ...(1,1),  laToken = ..."class".. y la = Token.CLASS
            Check(Token.CLASS);   //class ProgrPpal
            //Se cumple que:  (la == expected) => ejecuta Scan => token = ..."class"... y laToden = ..."ProgrPpal" 
            Code.Colorear("token");
            MessageBoxCon3Preg();//colorea "class" en ventana de Edicion
            //"Program = 'class' ident PosDeclars '{' MethodDeclsOpc '}'."
            //Ya reconoci� 'class', ahora va a reconocer ident
            program.Nodes.Add("ident");
            Parser.MessageBoxCon3Preg();
            Check(Token.IDENT); // "ProgrPpal" => debo insertar el token en la tabla de s�mbolos
            // es el comienzo del programa y abrir un nuevo alcance
            //Ahora token = "ProgrPpal" y laToken = "{"
            Code.Colorear("token");  //"class" ya lo pint�, ahora pinta "ProgrPpal"  (lo que hay en token)                                    
            MessageBoxCon3Preg();
            Symbol prog = Tab.Insert(Symbol.Kinds.Prog, token.str, Tab.noType);//lo cuelga de universe
            Code.CreateMetadata(prog);
            Tab.OpenScope(prog); //topScore queda ahora apuntando a un nuevo Scope
            //El Scope anterior (universo) lo accedo via topScore.outer
            //Ya analiz� Class ProgrPpal
            //Declaraciones (de ctes, de Globals(aunque diga de vars) y de clases) 
            //hasta q venga una "{"
            //"Program = 'class' ident PosDeclars '{' MethodDeclsOpc '}'."
            //Ya reconoci� ident, ahora va a reconocer PosDeclars
            System.Windows.Forms.TreeNode posDeclars = new System.Windows.Forms.TreeNode("PosDeclars");
            program.Nodes.Add(posDeclars);//Cuelga un TreeNode porque PosDeclars es No Terminal
            Parser.MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(1);  //"PosDeclars = . | Declaration PosDeclars.";
            Parser.MessageBoxCon3Preg();
            bool existeDecl = false;
            //"Declaration = ConstDecl | VarDecl | ClassDecl."
            while (la != Token.LBRACE && la != Token.EOF) //Si no existen declaraciones, la = Token.LBRACE
            {
                Code.Colorear("latoken"); //si existiera una declaracion, as "int i", colorea "int";  (yaPintado = true)
                //El argumento "false" => que no debe pintar el "token" (que en este caso seria "ProgrPpal"), sino el laToken (que es "int")
                //en este caso, debe "mirar hacia adelante" (laToken) 
                //para determinar la opcion de la produccion "PosDeclars = . | Declaration PosDeclars."    
                //Si laToken es "{" => la opcion es "PosDeclars = .", otherwise: "PosDeclars = Declaration PosDeclars."
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(2);
                System.Windows.Forms.TreeNode hijodeclar = new System.Windows.Forms.TreeNode("Declaration = ConstDecl | VarDecl | ClassDecl.");
                posDeclars.Nodes.Add(hijodeclar);
                MessageBoxCon3Preg();
                existeDecl = true;
                switch (la)
                {
                    case Token.CONST:
                        {
                            ConstDecl(hijodeclar);
                            break;
                        }
                    case Token.IDENT:  //Type ident..
                        {
                            Code.cargaProgDeLaGram("Declaration = VarDecl.");
                            System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("Declaration = VarDecl.");
                            hijodeclar.Nodes.Add(hijo1);
                            MessageBoxCon3Preg();
                            Code.seleccLaProdEnLaGram(6);
                            MessageBoxCon3Preg();
                            Code.cargaProgDeLaGram("VarDecl = Type  ident IdentifiersOpc ';'.");
                            Code.seleccLaProdEnLaGram(12);
                            MessageBoxCon3Preg();
                            Code.cargaProgDeLaGram("Type = ident LbrackOpc."); //ya pint� el ident (por ej "int en int ii);
                            VardDecl(Symbol.Kinds.Global, hijo1); //En program  //Table val;
                            break;
                        }
                    case Token.CLASS:
                        {
                            ClassDecl();/*No se encuentra la gramatica para implementar declaracione de clases" */
                            break;
                        }
                    default:
                        {
                            token = laToken;
                            Errors.Error("Se esperaba Const, Tipo, Class");
                            break;
                        }
                }

                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(1);
                MessageBoxCon3Preg();
                Code.cargaProgDeLaGram("selccion� la 1 PosDeclars = . | Declaration PosDeclars.");
            }
            Code.Colorear("latoken");
            MessageBoxCon3Preg();
            //en este caso, debe "mirar hacia adelante" (laToken) 
            //para determinar la opcion de la produccion "PosDeclars = . | Declaration PosDeclars."    
            //Si laToken es "{" => la opcion es "PosDeclars = .", otherwise: "PosDeclars = Declaration PosDeclars."
            if (!existeDecl)
            {
                posDeclars.Nodes.Add(".");
                posDeclars.ExpandAll(); //Visualiza (Expande) posDeclars
                Parser.MessageBoxCon3Preg();
            }
            if (ZZ.parser)
            {
                Console.WriteLine("Termin� con todas las declaraciones");
                Console.WriteLine("//el topScope queda apuntando a --> const size, class Table (con int[] pos y int[] neg), Table val");
            };
            if (ZZ.parser)
            {
                Console.WriteLine("empieza {"); if (ZZ.readKey) Console.ReadKey();
            }
            Check(Token.LBRACE);
            Code.Colorear("token");  //ya lo pinto
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(0);
            Parser.MessageBoxCon3Preg();
            program.Nodes.Add("'{'");
            program.ExpandAll();
            Parser.MessageBoxCon3Preg();
            System.Windows.Forms.TreeNode methodDeclsOpc = new System.Windows.Forms.TreeNode("MethodDeclsOpc");
            program.Nodes.Add(methodDeclsOpc);
            program.ExpandAll();
            Parser.MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(3);//3.MethodDeclsOpc = . | MethodDecl Meth
            Parser.MessageBoxCon3Preg();
            // si "la" pertenece a First(MethodDec) => s�lo deben haber metodos
            while ((la == Token.IDENT || la == Token.VOID) && la != Token.EOF)
            {
                MethodDecl(methodDeclsOpc);  //void Main() int x,i; {val = new Table;....}
                cantVarLocales = 0;
            }

            Check(Token.RBRACE);
            Code.Colorear("token");
            //////----------------------------------------------------------------Grupo 2 20/10/2015------------------------------------------------------
            MessageBoxCon3Preg(program);
            Code.seleccLaProdEnLaGram(0);
            MessageBoxCon3Preg();
            program.Nodes.Add("}");
            program.ExpandAll();
            MessageBoxCon3Preg();
            //////----------------------------------------------------------------Grupo 2 20/10/2015------------------------------------------------------
            if (ZZ.parser)
            {
                Console.WriteLine("antes de prog.locals = Tab.topScope.locals; Tab.CloseScope()");
                if (ZZ.readKey)
                    Console.ReadKey();
            };
            prog.locals = Tab.topScope.locals;
            if (ZZ.parser)
            {
                Console.WriteLine("mostrarSymbol");
                prog.mostrarSymbol("");
                if (ZZ.readKey) Console.ReadKey();
            };
            Tab.CloseScope();
            Tab.mostrarTab();
            bool Depuracion = false;
            if (!Depuracion)
                ParteFinal1();
            if (ZZ.parser)
            {
                Console.WriteLine("despues de prog.locals = Tab.topScope.locals; Tab.CloseScope()");
                if (ZZ.readKey) Console.ReadKey();
                Tab.mostrarTab();
            };
        }//Fin Program

        static void ConstDecl(System.Windows.Forms.TreeNode padre)  /////MALLLL
        {
            System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("Declaration = ConstDecl.");
            padre.Nodes.Add(hijo1);
            padre.ExpandAll();
            MessageBoxCon3Preg();
            System.Windows.Forms.TreeNode hijo2 = new System.Windows.Forms.TreeNode("ConstDecl = 'const' Type ident '=' NumberOrCharConst");
            hijo1.Nodes.Add(hijo2);
            hijo1.ExpandAll();
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(4);
            MessageBoxCon3Preg();

            Check(Token.CONST);  //const int i = 3;
            Code.Colorear("token");
            MessageBoxCon3Preg();
            hijo2.Nodes.Add("'const'");
            hijo2.ExpandAll();
            MessageBoxCon3Preg();
            Code.cargaProgDeLaGram("Declarations = Declaration Declarations.");
            Code.cargaProgDeLaGram("Declaration = ConstDecl.");
            //token=const  laToken=int
            Struct type;
            //= new Struct(Struct.Kinds.None); se podria haber hecho de 2 modos mas: ref y x valor
            // en ambos casos necesito Struct type = new...; 
            Type(out type); //En ConstDecl()  /
            hijo2.Nodes.Add("Type = " + type.kind.ToString());
            hijo2.ExpandAll();
            MessageBoxCon3Preg();

            if (type != Tab.intType && type != Tab.charType)
            {
                Errors.Error("el tipo de una def de const s�lo puede ser int o char");
            }
            Check(Token.IDENT);
            hijo2.Nodes.Add("ident");
            hijo2.ExpandAll();
            MessageBoxCon3Preg();
            Code.Colorear("token");
            if (muestraProducciones) MessageBoxCon3Preg(); ;
            //token qued� con i, laToken con =
            // debo agregar a la tabla de s�mbolos un nuevo s�mbolo  
            Symbol constante = Tab.Insert(Symbol.Kinds.Const, token.str, type);
            if (ZZ.parser)
                Console.WriteLine("termina de insertar constante con type = " + type.kind);
            Check(Token.ASSIGN);  //const
            hijo2.Nodes.Add("'='");
            hijo2.ExpandAll();
            MessageBoxCon3Preg();
            Code.Colorear("token"); if (muestraProducciones) MessageBoxCon3Preg();
            //token qued� con =, laToken con 10
            System.Windows.Forms.TreeNode hijo3 = new System.Windows.Forms.TreeNode("NumberOrCharConst");
            hijo2.Nodes.Add(hijo3);
            hijo2.ExpandAll();
            MessageBoxCon3Preg();
            switch (la)
            {
                case Token.NUMBER:
                    {
                        if (type != Tab.intType) Errors.Error("type debe ser int");
                        Check(Token.NUMBER);
                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        ////
                        hijo3.Nodes.Add("number");
                        hijo3.ExpandAll();
                        MessageBoxCon3Preg();
                        ////
                        if (muestraProducciones) MessageBoxCon3Preg();
                        constante.val = token.val;
                        break;
                    }
                case Token.CHARCONST:
                    {
                        if (type != Tab.charType) Errors.Error("type debe ser char");
                        Check(Token.CHARCONST);
                        hijo3.Nodes.Add("charConst");
                        hijo3.ExpandAll();
                        MessageBoxCon3Preg();
                        Code.Colorear("token"); if (muestraProducciones) MessageBoxCon3Preg();
                        constante.val = token.val;
                        break;
                    }
                default:
                    {
                        Errors.Error("def de const erronea");
                        break;
                    }
            }
            if (ZZ.parser)
                Console.WriteLine("Ahora actualiz� el valor");
            if (ZZ.parser)
                Tab.mostrarTab();
            Check(Token.SEMICOLON);
            Code.Colorear("token");
            MessageBoxCon3Preg();
            hijo2.Nodes.Add("';'");
            hijo2.ExpandAll();
            MessageBoxCon3Preg();
        }//Fin ConstDecl

        //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------------------         
        public static void Identifieropc(System.Windows.Forms.TreeNode identifieropc, Struct type, Symbol.Kinds kind)//NUEVA FUNCION RECURSIVA QUE CUELGA LOS IDENTIFIEROPC
        {
            if (la == Token.COMMA && la != Token.EOF)
            {
                MessageBoxCon3Preg();
                Scan();
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Code.cargaProgDeLaGram("IdentifiersOpc = ',' ident  IdentifiersOpc.");//deberia extender el arbol
                identifieropc.Nodes.Add("','");
                identifieropc.ExpandAll();
                if (muestraProducciones)
                    MessageBoxCon3Preg();
                Check(Token.IDENT); //otro identif
                
                Code.Colorear("token");
                if (muestraProducciones)
                    MessageBoxCon3Preg();

                identifieropc.Nodes.Add("ident");
                identifieropc.ExpandAll();
                MessageBoxCon3Preg();
                System.Windows.Forms.TreeNode identifieropc1 = new System.Windows.Forms.TreeNode("IdentifiersOpc");
                identifieropc.Nodes.Add(identifieropc1);
                identifieropc.ExpandAll();
                MessageBoxCon3Preg();
                //FGF INICIO 23/10
                cantVarLocales++;
                Symbol vble = Tab.Insert(kind, token.str, type);
                Code.CreateMetadata(vble);
                //FGF FIN  580 Identif
                Identifieropc(identifieropc1, type, kind);
            }
            else
            {
                MessageBoxCon3Preg();
                identifieropc.Nodes.Add(".");
                identifieropc.ExpandAll();
                MessageBoxCon3Preg();
            }
        }//Fin Identifieropc

        //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
        static void VardDecl(Symbol.Kinds kind, System.Windows.Forms.TreeNode padre)
        {  //visto  //si es "int[] pos" y viene de "Class Tabla {", kind es "Field"
            // si es Tabla val y viene de "class P {", kind es "Global"
            Struct type;// = new Struct(Struct.Kinds.None); //int i;
            Code.seleccLaProdEnLaGram(6);
            if (muestraProducciones) MessageBoxCon3Preg();
            Code.cargaProgDeLaGram("VarDecl = Type  ident IdentifiersOpc ';'");
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("Type");//type
            padre.Nodes.Add(hijo1);
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(12);
            MessageBoxCon3Preg();
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            Type(out type);  //En VardDecl
            //int[] en el caso del "int[] pos",... int, Table, Persona, int[], etc  
            //Table en el caso de Table val;
            //int en int x;
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            MessageBoxCon3Preg();
            System.Windows.Forms.TreeNode lbrakopc = new System.Windows.Forms.TreeNode("LbrakeOpc");
            hijo1.Nodes.Add(lbrakopc);
            hijo1.ExpandAll();
            MessageBoxCon3Preg();

            Code.seleccLaProdEnLaGram(13);
            MessageBoxCon3Preg();
            Code.Colorear("latoken");
            
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
 
            
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            MessageBoxCon3Preg();
            lbrakopc.Nodes.Add(".");
            lbrakopc.ExpandAll();
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(12);
            MessageBoxCon3Preg();
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            Check(Token.IDENT); // "pos", en int pos,   .....int,....  x, i, etc
            Code.seleccLaProdEnLaGram(6);
            MessageBoxCon3Preg();
            padre.Nodes.Add("ident");// Hace referencia a la x
            padre.ExpandAll();
            MessageBoxCon3Preg();
            Code.Colorear("token");
            MessageBoxCon3Preg();
            System.Windows.Forms.TreeNode hijo2 = new System.Windows.Forms.TreeNode("IdentifiersOpc");
            padre.Nodes.Add(hijo2);
            padre.ExpandAll();
            MessageBoxCon3Preg();
            //-------------------------------------------------Grupo 2 28/9/2015----------------------------------------------------------------------- 
            ////
            // debo insertar el token en la tabla de s�mbolos
            cantVarLocales++; //provisorio: esto deberia hacerlo solo para el caso de var locales (no para var globales)
            Symbol vble = Tab.Insert(kind, token.str, type);
            //vble no, poner simbolo (para pos, en int[] pos)
            //pues en este caso  es campo, y devuelve el Symbol p/pos,  type es int[]
            //puede ser val, en Tabla val, y type es Table //y devuelve el Symbol p/val
            Code.CreateMetadata(vble); //Para el campo pos (en int[] pos)Global, Field o .....  
            //o Para la vbe Global val
            //o para x en int x;
            Code.seleccLaProdEnLaGram(7);
            MessageBoxCon3Preg();
            Code.Colorear("latoken");
            Identifieropc(hijo2, type, kind);
            Check(Token.SEMICOLON);
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(6);
            MessageBoxCon3Preg();
            Code.Colorear("token");
            MessageBoxCon3Preg();
            padre.Nodes.Add("';'");
            padre.ExpandAll();
            MessageBoxCon3Preg();
            
        }//Fin VardDecl

        static void ClassDecl()
        {
            Check(Token.CLASS); //class Table {int[] pos;int[] pos;},.. class C1 {int i; void P1(){}; char ch; int P2{}; int[] arr;  }
            Check(Token.IDENT); // "Table", laToken queda con "{"
            String nombreDeLaClase = token.str;//Table
            Struct StructParaLaClase = new Struct(Struct.Kinds.Class);
            //crear un Struct para la clase (con kind=Class)

            Check(Token.LBRACE); //"{"... laToken queda con "Table",...const    C1,  o  Table (en class Table)   
            Symbol nodoClase = Tab.Insert(Symbol.Kinds.Type, nombreDeLaClase, //crea symbol p/la clase Table
                                          StructParaLaClase);  //Con type=class
            Code.CreateMetadata(nodoClase);//crea clase Din p/Table (por ej.), 
            //queda apuntada por nodoClase.type.sysType
            // todas las variables que declaren son tipo FIELD, excepto las clases (anidadas)
            //class C1 { => int i,j; char ch; Pers p=new Pers(); int P2{}; int[] arr;  }
            //class Table { => int[] pos; int[] neg}
            //por ahora, no permitimos void P1 {};
            Tab.OpenScope(nodoClase);
            while (la != Token.RBRACE && la != Token.EOF)  //itera p/c/campo (pos)
            {
                switch (la)
                {
                    case Token.CONST:
                        ConstDecl(null);  //const int size = 10
                        break;
                    case Token.IDENT:
                        {
                            //Type ident.., por ej: int i, int[] pos, etc...
                            //deberia ser declaracion de campo (pues cuelga de una clase, no de un metodo)
                            VardDecl(Symbol.Kinds.Field, null); // P/distinguir campos de clases
                            //Lo que va a declarar (pos), va a ser un Symbol con kind "field"
                            //int[] pos;
                            break;
                        }
                    case Token.CLASS:
                        {
                            ClassDecl();
                            break;
                        }
                    default:
                        {
                            Errors.Error("Se esperaba Const, tipo, class");
                            break;
                        }
                }
            }
            //crear un Struct para la clase (con kind=Class)
            //Struct StructParaLaClase = new Struct(Struct.Kinds.Class);
            //hacer StructParaLaClase.fields = topScope.locals
            StructParaLaClase.fields = Tab.topScope.locals;
            Tab.CloseScope();  //con lo cual recuperamos topSScope
            Check(Token.RBRACE);   //laToken queda con Table (en Table ...)
            //class C1 { => int i,j; char ch; Pers p=new Pers(); int P2{}; int[] arr;  }
            // int i,j; char ch; Pers p..etc, qued� apuntado por topScope.locals
        }//Fin ClassDecl

        /// G3 PERUMETHODDECL Arreglado los nombres de las variables.
        /// Corregido en PosDeclars ahora aparece '.'
        /// Codigo mas limpio.
        static void MethodDecl(System.Windows.Forms.TreeNode methodDeclsopc)
        {
            System.Windows.Forms.TreeNode methodDecl = new System.Windows.Forms.TreeNode("MethodDecl"); //cuelga ESTE NODO DESPUES DE pintar el void

            Struct type = new Struct(Struct.Kinds.None);
            System.Windows.Forms.TreeNode typeOrVoid = new System.Windows.Forms.TreeNode("TypeOrVoid"); //Pone por defecto void

            //Aca va haber problemas en un futuro si queremos agregar m�todos que devuelvan un "Type" de dato
            if (la == Token.VOID || la == Token.IDENT)
            {
                if (la == Token.VOID)
                {
                    Code.Colorear("latoken");
                    MessageBoxCon3Preg();
                    Check(Token.VOID); //token = void laToken = Main
                    ///// Agrega 'MethodDecl' al arbol y lo cuelga de MethodDeclsOpc
                    methodDeclsopc.Nodes.Add(methodDecl);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(8);
                    MessageBoxCon3Preg();
                    ///// Agrega 'TypeOrVoid' al arbol y lo cuelga de MethodDecl
                    methodDecl.Nodes.Add(typeOrVoid);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(9);
                    MessageBoxCon3Preg();
                    ///// Agrega 'void' al arbol y lo cuelga de typeorvoid
                    typeOrVoid.Nodes.Add("'void'");
                    typeOrVoid.ExpandAll();
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(8);
                    MessageBoxCon3Preg();
                    type = Tab.noType; //  para void
                }
                else if (la == Token.IDENT)
                    {
                        Type(out type);  //  token = UnTipo laToken = Main
                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        /////////// Agrega 'Type' al arbol y lo cuelga de typeorvoid
                        System.Windows.Forms.TreeNode ntype = new System.Windows.Forms.TreeNode("Type");
                        typeOrVoid.Nodes.Add(ntype);
                        MessageBoxCon3Preg();
                        ntype.Nodes.Add("" + type.kind.ToString());
                        ntype.ExpandAll();
                        MessageBoxCon3Preg();
                    }
                methodDecl.Nodes.Add("ident");
                methodDecl.ExpandAll();
                MessageBoxCon3Preg();
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Check(Token.IDENT);  //Main por ej.  //token = Main, laToken = "("
                Code.Colorear("token");
                MessageBoxCon3Preg();
                curMethod = Tab.Insert(Symbol.Kinds.Meth, token.str, type);//inserta void Main 
                Tab.OpenScope(curMethod);
                methodDecl.Nodes.Add("'('");
                MessageBoxCon3Preg();
                Check(Token.LPAR);  //Si Main() => no tiene FormPars
                MessageBoxCon3Preg();
                Code.Colorear("token");
                MessageBoxCon3Preg();
                ///// Agrega 'pars' a MethodDecl
                System.Windows.Forms.TreeNode pars = new System.Windows.Forms.TreeNode("Pars");
                methodDecl.Nodes.Add(pars);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(10);
                MessageBoxCon3Preg();
  
                if (la != Token.TYPE) //Antes la == Token.TYPE
                {
                    //infiere que no hay params => 1) debe venir un ")". 2) La pocion de la produccion es "."
                    //Aqui hay que crear los symbolos para los args

                    if (la != Token.RPAR) //A esto lo agregue para que cuando no se ponga un "TYPE" diga se esperaba un type
                        Check(Token.TYPE-1);


                    Code.Colorear("latoken");  //pinta el ")"
                    Check(Token.RPAR);
                    MessageBoxCon3Preg();
                    pars.Nodes.Add(".");
                    pars.ExpandAll();
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(8);
                    MessageBoxCon3Preg();
                    
                }
                else
                {
                    
                    System.Windows.Forms.TreeNode formPars = new System.Windows.Forms.TreeNode("FormPars");
                    pars.Nodes.Add(formPars);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(14);

                    FormPars(formPars);

                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(10);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(8);
                    MessageBoxCon3Preg();

                    Check(Token.RPAR); //Luego debe venir ")" al finalizar la lista de parametros
                }
 
                Code.Colorear("token");
                MessageBoxCon3Preg();
                methodDecl.Nodes.Add("')'");
                MessageBoxCon3Preg();

                //Comienza Nodo Declaration.
                Code.CreateMetadata(curMethod);
                //Comienza Block
                Block(methodDecl);  //Bloque dentro de MethodDecl()
                System.Windows.Forms.TreeNode block = new System.Windows.Forms.TreeNode("Block");
                methodDecl.Nodes.Add(block);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(16);
                MessageBoxCon3Preg();


                /*System.Windows.Forms.TreeNode posDeclars = new System.Windows.Forms.TreeNode("PosDeclars");
                methodDecl.Nodes.Add(posDeclars);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(1);
                MessageBoxCon3Preg();*/

                //bool encuentraDecl = false;
                /*Code.CreateMetadata(curMethod);  //genera il
                    //Declaraciones  por ahora solo decl de var, luego habria q agregar const y clases
                    while (la != Token.LBRACE && la != Token.EOF)
                    //void Main()==> int x,i; {val = new Table;....}
                    {
                        if (la != Token.IDENT)  //debe venir un tipo (int por ej)
                        {
                            //encuentraDecl = true;
                            Code.Colorear("latoken"); //colorea "int"  en int i; 
                            //Infiere la 2� opcion de PosDeclars   aaaaaaaa
                            System.Windows.Forms.TreeNode declaration = new System.Windows.Forms.TreeNode("Declaration");
                            posDeclars.Nodes.Add(declaration);
                            posDeclars.ExpandAll();
                            MessageBoxCon3Preg();
                            Code.seleccLaProdEnLaGram(2);
                            System.Windows.Forms.TreeNode varDecl = new System.Windows.Forms.TreeNode("VarDecl");
                            declaration.Nodes.Add(varDecl);
                            declaration.ExpandAll();
                            MessageBoxCon3Preg();
                            Code.seleccLaProdEnLaGram(6);
                            VardDecl(Symbol.Kinds.Local, varDecl); // int x,i; en MethodDecl()  con int ya consumido
                        }
                        else
                        {
                            token = laToken;
                            Errors.Error("espero una declaracion de variable");
                        }
                    }
                    //Termina Vardecl.
                Code.seleccLaProdEnLaGram(2);*/


                curMethod.nArgs = Tab.topScope.nArgs;
                curMethod.nLocs = Tab.topScope.nLocs;
                curMethod.locals = Tab.topScope.locals;
                Tab.CloseScope();
                Tab.mostrarTab();
                Code.il.Emit(Code.RET);  //si lo saco se clava en el InvokeMember
                Parser.nroDeInstrCorriente++;
                Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.ret;
                Code.cargaInstr("ret");

                //Aca al modificar la regla (8) nunca voy a "PosDeclars" despu�s.

                /*Code.seleccLaProdEnLaGram(1);
                MessageBoxCon3Preg();
                System.Windows.Forms.TreeNode posDeclarsAux = new System.Windows.Forms.TreeNode("PosDeclars");
                posDeclarsAux.Nodes.Add(".");
                posDeclarsAux.ExpandAll();
                methodDecl.Nodes.Add(posDeclarsAux);*/
                Code.Colorear("latoken");  //"}"
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(8);
                MessageBoxCon3Preg();

                //Comienza Block
                /*Block(methodDecl);  //Bloque dentro de MethodDecl() 
                curMethod.nArgs = Tab.topScope.nArgs;
                curMethod.nLocs = Tab.topScope.nLocs;
                curMethod.locals = Tab.topScope.locals;
                Tab.CloseScope();
                Tab.mostrarTab();
                Code.il.Emit(Code.RET);  //si lo saco se clava en el InvokeMember
                Parser.nroDeInstrCorriente++;
                Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.ret;
                Code.cargaInstr("ret");*/
            }
        }//Fin MethodDecl

        static void FormPars(System.Windows.Forms.TreeNode padre)//Falta �rbol
        {
            Struct type = new Struct(Struct.Kinds.None);

            if(la == Token.TYPE)
            {
                MessageBoxCon3Preg();
                Type(out type); // pasa a ser token = "type" y laToken="ident" porque dentro de "Type" se hace un "CHECK"

                Symbol vble = Tab.Insert(Symbol.Kinds.Arg, laToken.str, type);
                Code.CreateMetadata(vble);
                
                System.Windows.Forms.TreeNode typeRule = new System.Windows.Forms.TreeNode("Type");
                padre.Nodes.Add(typeRule);
                MessageBoxCon3Preg();
                
 
                System.Windows.Forms.TreeNode lbrackOpc = new System.Windows.Forms.TreeNode("LbrackOpc");
                typeRule.Nodes.Add(lbrackOpc);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(13);
                MessageBoxCon3Preg();
                Code.cargaProgDeLaGram("LbrackOpc = . | \"[\" \"].");

                if (token.kind == Token.RBRACK) //Si token.kind=Token.RBRACk quiere decir que el argumento fue un arreglo.
                {
                    
                    lbrackOpc.Nodes.Add("[");
                    lbrackOpc.ExpandAll();
                    MessageBoxCon3Preg();

                    lbrackOpc.Nodes.Add("]");
                    MessageBoxCon3Preg();
                }
                else
                {
                    lbrackOpc.Nodes.Add(".");
                    lbrackOpc.ExpandAll();
                    MessageBoxCon3Preg();
                }

                
                Check(Token.IDENT);
                Code.Colorear("token");
                MessageBoxCon3Preg();

                Code.seleccLaProdEnLaGram(12);
                MessageBoxCon3Preg();

                padre.Nodes.Add("ident"); //Aca token="ident" al 1� par�metro digamos
                padre.ExpandAll();
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(14);
                MessageBoxCon3Preg();


                Code.Colorear("latoken"); //Colorear de Verde ")" o "," porque no sabemos que viene en regla 15
                MessageBoxCon3Preg();


                System.Windows.Forms.TreeNode commaTypeIdentOpc = new System.Windows.Forms.TreeNode("CommaTypeIdentOpc");
                padre.Nodes.Add(commaTypeIdentOpc);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(15);
                MessageBoxCon3Preg();
                Code.cargaProgDeLaGram("CommaTypeIdentOpc = . | , Type ident CommaTypeIdentOpc.");
               
                while (la == Token.COMMA && la != Token.EOF && la != Token.RPAR)
                {
                    commaTypeIdentOpc.Nodes.Add(",");
                    commaTypeIdentOpc.ExpandAll();
                    MessageBoxCon3Preg();

                    Check(Token.COMMA);

                    Code.Colorear("token");
                    MessageBoxCon3Preg();

                    Type(out type);

                    vble = Tab.Insert(Symbol.Kinds.Arg, laToken.str, type);
                    Code.CreateMetadata(vble);

                    MessageBoxCon3Preg();
                    System.Windows.Forms.TreeNode typeRule2 = new System.Windows.Forms.TreeNode("Type");
                    commaTypeIdentOpc.Nodes.Add(typeRule2);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(12);
                    MessageBoxCon3Preg();
                    Code.Colorear("token");
                    MessageBoxCon3Preg();

                    System.Windows.Forms.TreeNode lbrackOpc2 = new System.Windows.Forms.TreeNode("LbrackOpc");
                    typeRule2.Nodes.Add(lbrackOpc2);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(13);
                    Code.cargaProgDeLaGram("LbrackOpc = . | \"[\" \"].");

                    MessageBoxCon3Preg();
                    Code.Colorear("latoken"); //viene "." o "["?
                    MessageBoxCon3Preg();

                    if (token.kind == Token.RBRACK) //Si token.kind=Token.RBRACk quiere decir que el argumento fue un arreglo.
                    {

                        lbrackOpc2.Nodes.Add("[");
                        lbrackOpc2.ExpandAll();
                        MessageBoxCon3Preg();

                        lbrackOpc2.Nodes.Add("]");
                        MessageBoxCon3Preg();
                    }
                    else
                    {
                        lbrackOpc2.Nodes.Add(".");
                        lbrackOpc2.ExpandAll();
                        MessageBoxCon3Preg();
                    }

                    Check(Token.IDENT);
                    Code.Colorear("token");
                    MessageBoxCon3Preg();

                    Code.seleccLaProdEnLaGram(12);
                    MessageBoxCon3Preg();

                    commaTypeIdentOpc.Nodes.Add("ident");
                    MessageBoxCon3Preg();

                    Code.seleccLaProdEnLaGram(15);
                    MessageBoxCon3Preg();

                    Code.Colorear("latoken"); //Colorear de Verde ")" o "," porque no sabemos que viene en regla 15
                    MessageBoxCon3Preg();

                }//Fin while

                Code.seleccLaProdEnLaGram(15);
                MessageBoxCon3Preg();
                Code.cargaProgDeLaGram("CommaTypeIdentOpc = .");

                commaTypeIdentOpc.Nodes.Add(".");
                commaTypeIdentOpc.ExpandAll();
                MessageBoxCon3Preg();

                Code.seleccLaProdEnLaGram(14);
                MessageBoxCon3Preg();

            }
        }//Fin FormPars

        static void Type(out Struct xType)
        {
            Code.seleccLaProdEnLaGram(12);
            Code.cargaProgDeLaGram("Type = LbrackOpc.");
            if (la != Token.TYPE)  //debe venir un tipo (int por ej)
            {
                Errors.Error("espera un tipo");
                xType = Tab.noType;
            }
            else
            {
                Check(Token.TYPE); //=> token=int y laToken=[,  .....token=int y laToken=size, en int size 
                Code.Colorear("token"); //si viene de... yaPintado = true => no pinta nada
                MessageBoxCon3Preg();

                Symbol sym = Tab.Find(token.str);  //busca int  y devuelve el Symbol p/int
                //Busca Table y devuelve el Symbol p/Table
                if (ZZ.parser)
                    Console.WriteLine("Tab.Find(" + token.str + ") =>" + sym.ToString() + "..."); //if (ZZ.readKey) Console.ReadKey();
                if (sym == null)
                {
                    Errors.Error("debe venir un tipo");//Fran
                    xType = Tab.noType;
                }
                else
                {
                    xType = sym.type; //Devuelve int como tipo (Struct), no como nodo Symbol 
                };

                Code.Colorear("latoken"); //un "[" o lo que sigue al type (un ident en int ident1)
                if (la == Token.LBRACK)  // 
                {
                    MessageBoxCon3Preg();
                    Check(Token.LBRACK);
                    Check(Token.RBRACK);
                    Code.Colorear("token");
                    MessageBoxCon3Preg();
                    Code.cargaProgDeLaGram("LbrackOpc = '[' ']'.");
                    MessageBoxCon3Preg();
                    //int tipo del elem del array
                    xType = new Struct(Struct.Kinds.Arr, sym.type);
                    //podria haber sido xType (Struct del int) en vez de sym.type  
                    //el nuevo xType que obtiene es un array de int
                }
                else Code.cargaProgDeLaGram("LbrackOpc = .");
            }
        }//Fin Type 

        static void markLabelMio(int nroInstrParaRectificarElIf, System.Windows.Forms.TreeNode padrecito)
        {
            Parser.nroDeInstrCorriente++;
            Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.nop;
            Code.cargaInstr("nop   ");
            //ir a la linea correspondiente y marcar el label con la instr corriente
            Parser.cil[nroInstrParaRectificarElIf].nroLinea = nroDeInstrCorriente;//Br cond ahora est� definido
            Parser.cil[nroInstrParaRectificarElIf].instrString =
                Parser.cil[nroInstrParaRectificarElIf].instrString.Substring(0,
                    Parser.cil[nroInstrParaRectificarElIf].instrString.Length - 2)
                    + nroDeInstrCorriente.ToString();
            //reescribe cil (nuevoRichTexBox3) rectificando la marca
            string newRichTexBox3 = cil[0].instrString;
            for (int i = 1; i <= nroDeInstrCorriente; i++)
            {
                newRichTexBox3 = newRichTexBox3 + "\n" + cil[i].instrString;
            }
            Program1.form1.richTextBox3.Text = newRichTexBox3;
        }//Fin markLabelMio

        // G3 PERUSTATEMENT arreglo nombres de variables (padre e hijos) en este metodo.
        // Nodos con palabras reservadas empiezan con n, ej. nodo while: nwhie.
        // O cuando hay varios, ej. varios statements: nstatement, nstatement2.
        static void Statement(System.Windows.Forms.TreeNode statement)
        {
            if (ZZ.ParserStatem) Console.WriteLine("Comienza statement:" + laToken.str);
            if (la == Token.IDENT)
            {
                MessageBoxCon3Preg();
                Code.Colorear("token"); //laToken (ident)  "writeln"  ya pintado  o "var1" en var1 = 10;
                Item itemIzq, itemDer; // = new Item();  // 

                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(31);  //scroll needed
                MessageBoxCon3Preg();
                //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
                System.Windows.Forms.TreeNode designator = new System.Windows.Forms.TreeNode("Designator");
                statement.Nodes.Add(designator);
                MessageBoxCon3Preg(statement);
                //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
                Designator(out itemIzq, designator); //val   en statement
                String parteFinalDelDesign = token.str;
                //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
                System.Windows.Forms.TreeNode RestOfstatement = new System.Windows.Forms.TreeNode("RestOfStatement");
                Code.seleccLaProdEnLaGram(18);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(22);
                MessageBoxCon3Preg();
                Code.Colorear("latoken");
                MessageBoxCon3Preg();
                statement.Nodes.Add(RestOfstatement);
                MessageBoxCon3Preg(statement);
                //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
                if (ZZ.parser)
                    Console.WriteLine("pasa el Designator()");
                switch (la)
                {
                    case Token.ASSIGN:  //asignacion  Designator = ....
                        {
                            Check(Token.ASSIGN);

                            //-------------------------------------------------Grupo 2 30/9/2015-----------------------------------------------------------
                            RestOfstatement.Nodes.Add("'='");
                            RestOfstatement.ExpandAll();
                            MessageBoxCon3Preg(RestOfstatement);
                            MessageBoxCon3Preg();
                            System.Windows.Forms.TreeNode nexpr = new System.Windows.Forms.TreeNode("Expr");
                            RestOfstatement.Nodes.Add(nexpr);
                            RestOfstatement.ExpandAll();
                            MessageBoxCon3Preg();
                            Code.seleccLaProdEnLaGram(23);
                            MessageBoxCon3Preg(RestOfstatement);
                            //-------------------------------------------------Grupo 2 30/9/2015-----------------------------------------------------------
                            Expr(out itemDer, nexpr);

                            MessageBoxCon3Preg();
                            RestOfstatement.Nodes.Add(";");                        
                            MessageBoxCon3Preg();

                            Code.Load(itemDer);
                            Code.Assign(itemIzq, itemDer, nexpr);
                            if (ZZ.parser)
                            {
                                Console.WriteLine("Termina statement de asign: ..." + parteFinalDelDesign
                                    + " = ....." + Token.names[token.kind] + " str=" + token.str);
                                if (ZZ.readKey) Console.ReadKey();
                            };
                            break;
                        }
                    case Token.LPAR:   //Designator(....  metodo(.....
                        {
                            Check(Token.LPAR);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            if (la == Token.MINUS || la == Token.IDENT ||
                                la == Token.NUMBER || la == Token.CHARCONST ||
                                la == Token.NEW || la == Token.LPAR)
                                ActPars();

                            MessageBoxCon3Preg();
                            Check(Token.RPAR);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            break;
                        }
                    case Token.PPLUS: // Designator++   var++
                        {
                            Check(Token.PPLUS);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            RestOfstatement.Nodes.Add("'++'");
                            RestOfstatement.ExpandAll();
                            MessageBoxCon3Preg(RestOfstatement);
                            if (ZZ.parser) Console.WriteLine("reconoci� el ++"); //zzzzzz
                            break;
                        }
                    case Token.MMINUS:
                        {
                            Check(Token.MMINUS);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            RestOfstatement.Nodes.Add("'--'");
                            RestOfstatement.ExpandAll();
                            MessageBoxCon3Preg(RestOfstatement);
                            break;
                        }
                }
                Check(Token.SEMICOLON);//falta nodo aca//
                Code.seleccLaProdEnLaGram(22);
                MessageBoxCon3Preg();
                Code.Colorear("token");
                MessageBoxCon3Preg();
            }
            else
            {
                Item item, itemAncho;
                switch (la)
                {
                    case Token.IF:
                        int nroInstrParaRectificarElIf;
                        Item x; Label end;
                        Check(Token.IF);
                        System.Windows.Forms.TreeNode If = new System.Windows.Forms.TreeNode("'if'");
                        statement.Nodes.Add(If);
                        MessageBoxCon3Preg(statement);
                        Check(Token.LPAR);
                        If.Nodes.Add("(");
                        If.ExpandAll();
                        MessageBoxCon3Preg(If);
                        Condition(out x);
                        System.Windows.Forms.TreeNode condition = new System.Windows.Forms.TreeNode("Condition");
                        If.Nodes.Add(condition);
                        MessageBoxCon3Preg(If);
                        if (ZZ.parser) Console.WriteLine("reconoci� la cond del if");
                        Check(Token.RPAR);
                        If.Nodes.Add(")");
                        MessageBoxCon3Preg(If);
                        Code.FJump(x);  //dentro del if (i<10) 
                        //Code.FJump(x) => bge x.fLabel, i.e. si >= debe ir a x.fLabel (debe saltar el then)
                        //x.fLabel est� indefinido, luego habr� un  //luego habr� un Code.il.MarkLabel(x.fLabel);

                        //Code.FJump(x)=> Parser.cil[Parser.nroDeInstrCorriente]...=> bge -1 (por ej) 
                        nroInstrParaRectificarElIf = Parser.nroDeInstrCorriente;
                        //luego habr� un Parser.cil[nroInstrParaRectificarElIf].nroLinea = label...
                        //reemplazando "bge -1" con "bge label..."
                        //////////
                        System.Windows.Forms.TreeNode nstatement = new System.Windows.Forms.TreeNode("Statement");
                        If.Nodes.Add(nstatement);
                        MessageBoxCon3Preg(If);
                        /////////
                        Statement(nstatement); //en el if de una statement
                        if (ZZ.parser) Console.WriteLine("reconoci� la Statem del if");
                        if (la == Token.ELSE)
                        {
                            Check(Token.ELSE);
                            ///////////
                            System.Windows.Forms.TreeNode nelse = new System.Windows.Forms.TreeNode("Else");
                            If.Nodes.Add(nelse);
                            MessageBoxCon3Preg(If);
                            //////////
                            end = Code.il.DefineLabel();
                            int endMio = -1;
                            Code.Jump(end);  //=> il.Emit(BR, end); 
                            Parser.nroDeInstrCorriente++;
                            Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.branchInc;
                            Parser.cil[Parser.nroDeInstrCorriente].nroLinea = endMio;  //Br Incond est� indefinido
                            Code.cargaInstr("br" + "  " + Parser.cil[Parser.nroDeInstrCorriente].nroLinea.ToString());
                            //br end => br -1
                            int nroInstrParaRectificarElEndDelIf = Parser.nroDeInstrCorriente;

                            Code.il.MarkLabel(x.fLabel);
                            markLabelMio(nroInstrParaRectificarElIf, nelse);//agregar una nop
                            //m�s: ir a la linea correspondiente y marcar el label con la instr corriente
                            //m�s: reescribe cil (nuevoRichTexBox3) rectificando la marca

                            Statement(nelse);  //en el else de una statemnet
                            if (ZZ.parser) Console.WriteLine("reconoci� la Statem del else del if");
                            Code.il.MarkLabel(end);
                            markLabelMio(nroInstrParaRectificarElEndDelIf, nelse);//agregar una nop
                            //m�s: ir a la linea correspondiente y marcar el label con la instr corriente
                            //m�s: reescribe cil (nuevoRichTexBox3) rectificando la marca
                        }
                        else
                        {
                            Code.il.MarkLabel(x.fLabel);
                            markLabelMio(nroInstrParaRectificarElIf, nstatement);//agregar una nop
                            //m�s: ir a la linea correspondiente y marcar el label con la instr corriente
                            //m�s: reescribe cil (nuevoRichTexBox3) rectificando la marca
                        }
                        break;
                    /*case Token.TYPE:
                        
                        System.Windows.Forms.TreeNode posDeclars = new System.Windows.Forms.TreeNode("PosDeclars");
                        statement.Nodes.Add(posDeclars);
                        MessageBoxCon3Preg(statement);
                        Code.seleccLaProdEnLaGram(1);
                        MessageBoxCon3Preg();
                        //Code.CreateMetadata(curMethod);

                        while (la != Token.LBRACE && la != Token.EOF)
                        //void Main()==> int x,i; {val = new Table;....}
                        {
                            if (la == Token.TYPE)
                            {
                                //encuentraDecl = true;
                                Code.Colorear("latoken"); //colorea "int"  en int i; 
                                                          //Infiere la 2� opcion de PosDeclars   aaaaaaaa
                                System.Windows.Forms.TreeNode declaration = new System.Windows.Forms.TreeNode("Declaration");
                                posDeclars.Nodes.Add(declaration);
                                posDeclars.ExpandAll();
                                MessageBoxCon3Preg();
                                Code.seleccLaProdEnLaGram(2);
                                System.Windows.Forms.TreeNode varDecl = new System.Windows.Forms.TreeNode("VarDecl");
                                declaration.Nodes.Add(varDecl);
                                declaration.ExpandAll();
                                MessageBoxCon3Preg();
                                Code.seleccLaProdEnLaGram(6);
                                VardDecl(Symbol.Kinds.Local, varDecl); // int x,i; en MethodDecl()  con int ya consumido

                                
                            }
                            else
                                break;

                        }

                        //Termina Vardecl.
                        if (cantVarLocales > 0)
                        {
                            string instrParaVarsLocs = ".locals init(int32 V_0";
                            for (int i = 1; i < cantVarLocales; i++)
                            {
                                instrParaVarsLocs = instrParaVarsLocs + "," + "\n          int32 V_" + i.ToString(); // +"  ";
                            }
                            instrParaVarsLocs = instrParaVarsLocs + ")";
                            Code.cargaInstr(instrParaVarsLocs);

                        }

                        Code.seleccLaProdEnLaGram(2);
                        MessageBoxCon3Preg();

                        Code.seleccLaProdEnLaGram(1);
                        MessageBoxCon3Preg();
                        System.Windows.Forms.TreeNode posDeclarsAux = new System.Windows.Forms.TreeNode("PosDeclars");
                        posDeclarsAux.Nodes.Add(".");
                        posDeclarsAux.ExpandAll();
                        posDeclars.Nodes.Add(posDeclarsAux);
                        Code.Colorear("latoken");  
                        Code.seleccLaProdEnLaGram(18);
                        MessageBoxCon3Preg();
                        Code.seleccLaProdEnLaGram(17);
                        MessageBoxCon3Preg();
                        break;
                    */

                    case Token.WHILE:
                        //Item x;
                        int nroInstrParaRectificarElWhile;
                        Check(Token.WHILE);
                        System.Windows.Forms.TreeNode nwhile = new System.Windows.Forms.TreeNode("'While'");
                        statement.Nodes.Add(nwhile);
                        MessageBoxCon3Preg(statement);
                        Label top = Code.il.DefineLabel();
                        int topMio = -1;
                        Code.il.MarkLabel(top);
                        topMio = Parser.nroDeInstrCorriente + 1; //(instr sig a la actual)
                        //para luego..Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.branchInc;
                        //Parser.cil[Parser.nroDeInstrCorriente].nroLinea = topMio;  //Br Incond est� definido
                        Check(Token.LPAR);
                        Condition(out x);
                        nwhile.Nodes.Add("'('");
                        nwhile.ExpandAll();
                        MessageBoxCon3Preg(nwhile);
                        nwhile.Nodes.Add("Condition");
                        MessageBoxCon3Preg(nwhile);
                        if (ZZ.parser) { Console.WriteLine("Termina con la cond del while"); };
                        Check(Token.RPAR);
                        MessageBoxCon3Preg(nwhile);
                        Code.FJump(x); //dentro del while (i<10) 
                        //Code.FJump(x) => bge x.fLabel, i.e. si >= debe ir a x.fLabel (debe salir del loop)
                        //x.fLabel est� indefinido, luego habr� un  //luego habr� un Code.il.MarkLabel(x.fLabel);
                        //Code.FJump(x)=> Parser.cil[Parser.nroDeInstrCorriente]...=> bge -1 (por ej) 
                        nroInstrParaRectificarElWhile = Parser.nroDeInstrCorriente;
                        //luego habr� un Parser.cil[nroInstrParaRectificarElWhile].nroLinea = label...
                        //reemplazando "bge -1" con "bge label..."
                        Check(Token.LBRACE);
                        nwhile.Nodes.Add("'{'");
                        MessageBoxCon3Preg(nwhile);
                        //cuerpo del while  
                        while (la != Token.RBRACE && la != Token.EOF)
                        {
                            System.Windows.Forms.TreeNode nstatement2 = new System.Windows.Forms.TreeNode("Statement");
                            nwhile.Nodes.Add(nstatement2);
                            MessageBoxCon3Preg(nwhile);
                            Statement(nstatement2);
                        } //dentro del while
                        Check(Token.RBRACE);
                        nwhile.Nodes.Add("'}'");
                        MessageBoxCon3Preg(nwhile);
                        if (ZZ.parser) { Console.WriteLine("Termina statement while"); if (ZZ.readKey) Console.ReadKey(); };
                        Code.Jump(top);
                        Parser.nroDeInstrCorriente++;
                        Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.branchInc;
                        Parser.cil[Parser.nroDeInstrCorriente].nroLinea = topMio;  //Br Incond est� definido
                        Code.cargaInstr("br" + "  " + Parser.cil[Parser.nroDeInstrCorriente].nroLinea.ToString());
                        //br top
                        Code.il.MarkLabel(x.fLabel);
                        //agregar una nop
                        Parser.nroDeInstrCorriente++;
                        Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.nop;
                        //Parser.cil[Parser.nroDeInstrCorriente].instrString =
                        Code.cargaInstr("nop   ");
                        //ir a la linea correspondiente y marcar el label con la instr corriente
                        Parser.cil[nroInstrParaRectificarElWhile].nroLinea = nroDeInstrCorriente;//Br cond ahora est� definido
                        Parser.cil[nroInstrParaRectificarElWhile].instrString =
                            Parser.cil[nroInstrParaRectificarElWhile].instrString.Substring(0,
                                Parser.cil[nroInstrParaRectificarElWhile].instrString.Length - 2)
                                + nroDeInstrCorriente.ToString();
                        Program1.form1.richTextBox3.Text = "";
                        //System.Windows.Forms.MessageBox.Show("empieza......");
                        //reescribe cil (nuevoRichTexBox3) rectificando la marca
                        string nuevoRichTexBox3 = cil[0].instrString;
                        for (int i = 1; i <= nroDeInstrCorriente; i++)
                        {
                            nuevoRichTexBox3 = nuevoRichTexBox3 + "\n" + cil[i].instrString;
                        }
                        Program1.form1.richTextBox3.Text = nuevoRichTexBox3;
                        break;
                    case Token.BREAK:
                        Check(Token.BREAK);
                        Check(Token.SEMICOLON);
                        break;
                    case Token.RETURN:
                        Check(Token.RETURN);
                        // First(Expr)
                        if (la == Token.MINUS || la == Token.IDENT || la == Token.NUMBER ||
                            la == Token.CHARCONST || la == Token.NEW || la == Token.LPAR)
                            Expr(out item, null);//Falta procesar
                        Check(Token.SEMICOLON);
                        break;
                    case Token.READ:
                        Check(Token.READ);
                        Check(Token.LPAR);
                        if (la == Token.IDENT)
                            Designator(out item, null);//En el READ
                        Check(Token.RPAR);
                        Check(Token.SEMICOLON);
                        //ZZ.parser = true; 
                        if (ZZ.parser) Console.WriteLine("reconoci� el Read");
                        break;
                    case Token.WRITELN: /// En Statement G3 PERUWRITELN
                        //token queda con ";" y laToken = WRITELN y ch con '('               
                        ///////////////////////// Este bloque agrega y muestra writeln en el arbol y selecciona la gramatica
                        Check(Token.WRITELN);
                        System.Windows.Forms.TreeNode writeln = new System.Windows.Forms.TreeNode("'writeln'");
                        statement.Nodes.Add(writeln);
                        MessageBoxCon3Preg(statement);
                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        ////////////////////////// 

                        //token queda con WRITELN y laToken =  "(" y ch con Comm Doble
                        //equivalente a Check(Token.LPAR);
                        //debe quedar token = "("  y laToken = "texto posiblem vacio"               
                        if (la == Token.LPAR) //ch = Comm Doble
                        {
                            //Scan(); //token = "("
                            ////////////////////////// Agrega '(' al arbol y lo muestra
                            writeln.Nodes.Add("'('");
                            writeln.ExpandAll(); // G3 2015
                            MessageBoxCon3Preg(writeln);
                            //////////////////////////
                            Code.Colorear("latoken"); //pinta el "("
                            MessageBoxCon3Preg();
                            Code.Colorear("token"); //solo para que deje yaPintado en false
                            MessageBoxCon3Preg();

                            if (Scanner.ch != '\"')
                                Errors.Error("Se esperaba una COMILLA DOBLE");
                            else
                            {
                                string argStr = "";
                                Scanner.NextCh(); //ch = 2� com doble o Primer char del argStr

                                while (Scanner.ch != '\"')
                                {
                                    //if (ch == EOF) return new Token(Token.EOF, line, col);
                                    argStr = argStr + Scanner.ch.ToString();
                                    Scanner.NextCh();  //ch = 2� char del argStr o Com Doble
                                }
                                //ch = Com Doble
                                token = new Token(Scanner.line, Scanner.col);
                                token.kind = Token.COMILLADOBLE; //se va a llamar argDeWriteLine
                                token.str = argStr; // excluye las comillas dobles
                                //token.line lo deja =
                                token.col = token.col - argStr.Length; //+ 1; // -3; //DESPUES DEL "("

                                ////////////////////////////  Agrega 'argString' al arbol y lo muestra 
                                writeln.Nodes.Add("argstring"); // G3 2015
                                MessageBoxCon3Preg(writeln);
                                Code.Colorear("token");
                                MessageBoxCon3Preg();

                                ////////////////////////////
                                Parser.nroDeInstrCorriente++;
                                //Agregar  ldString  argStr, 
                                Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.ldstr;
                                Parser.cil[Parser.nroDeInstrCorriente].argDelWriteLine = argStr;

                                muestraCargaDeInstrs = false; // -- G3 -  (Es para que no muestre la pantalla)
                                Code.cargaInstr("ldstr \"" + argStr + "\" ");
                                muestraCargaDeInstrs = true; // -- G3 -

                                Parser.nroDeInstrCorriente++;
                                Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.writeln;
                                muestraCargaDeInstrs = false; // -- G3 - (Es para que no muestre la pantalla)
                                Code.cargaInstr("call writeln#(string)");
                                muestraCargaDeInstrs = true; // -- G3 -
                                //Parser.cil[Parser.nroDeInstrCorriente].nro = item.val; // item.val;           //item.val;  aaaaaaa 
                                //Parser.cil[Parser.nroDeInstrCorriente].argDelWriteLine = argStr;  //Provisorio ya no se usa argDelWriteLine


                                Scanner.NextCh(); //ch=")"
                                token = laToken; //token queda con argDeWriteLine
                                laToken = Scanner.Next(); //la token queda con ")"
                                la = laToken.kind;
                                Code.il.EmitWriteLine(argStr);

                                ////////////////////////////// Agrega ')' al arbol y lo muestra

                                writeln.Nodes.Add("')'");
                                MessageBoxCon3Preg(writeln);

                                Check(Token.RPAR);
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                                /////////////////////////////  Agrega ';' al arbol y lo muestra

                                writeln.Nodes.Add("';'"); 
                                MessageBoxCon3Preg(writeln);
                                Check(Token.SEMICOLON);
                                MessageBoxCon3Preg();
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                            }
                        }

                        break;
                    case Token.WRITE: ///  En Statement

                        // G3 - PERUWRITE
                        //////////////// Agrega 'write' al arbol y lo muestra y colorea
                        Check(Token.WRITE);
                        System.Windows.Forms.TreeNode write = new System.Windows.Forms.TreeNode("'write'");
                        statement.Nodes.Add(write);
                        MessageBoxCon3Preg(statement);

                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        /////////////////
                        ///////////////// Agrega '(' al arbol y lo muestra y colorea
                        Check(Token.LPAR);
                        statement.Nodes.Add("'('");
                        MessageBoxCon3Preg(statement);

                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        ////////////////
                        //////////////// Agrega 'Expr' al arbol y lo muestra y colorea y selecciona la regla de Expr
                        System.Windows.Forms.TreeNode expr1 = new System.Windows.Forms.TreeNode("Expr");
                        statement.Nodes.Add(expr1);
                        MessageBoxCon3Preg(statement);
                        Code.seleccLaProdEnLaGram(23);
                        MessageBoxCon3Preg();
                        Code.Colorear("latoken");
                        MessageBoxCon3Preg();
                        ////////////////

                        // First(Expr)
                        if (la == Token.MINUS || la == Token.IDENT || la == Token.NUMBER ||
                            la == Token.CHARCONST || la == Token.NEW || la == Token.LPAR)
                            //Code.il.EmitWriteLine("dentro del WRITE, antes del Expr(out item)");

                            AA(out item); ///?????

                        Expr(out item, expr1); // Crea la Expr
                        System.Windows.Forms.TreeNode expr2 = new System.Windows.Forms.TreeNode("Expr");
                        if (la == Token.RBRACE) // Le provee 10 espacios de escritura sino manda ningun numero
                        // Por ej. write(x);
                        {
                            statement.Nodes.Add("')'");
                            MessageBoxCon3Preg(statement);
                            Code.Colorear("latoken");
                            
                        }
                        else
                        {
                            /////////////// Agrega ',' al arbol y lo muestra
                            Check(Token.COMMA);
                            Code.seleccLaProdEnLaGram(18);
                            MessageBoxCon3Preg();
                            write.Nodes.Add("','");
                            MessageBoxCon3Preg(write);
                            ///////////////


                            /////////////// Agrega 'Expr' al arbol y lo muestra y seecciona la regla de Expr

                            write.Nodes.Add(expr2);
                            MessageBoxCon3Preg(write);
                            Code.seleccLaProdEnLaGram(23);
                            MessageBoxCon3Preg();
                            //////////////

                            Expr(out itemAncho, expr2);
                            Code.il.EmitCall(Code.CALL, Code.writeInt, null); //Probar   provisorio

                            Parser.nroDeInstrCorriente++;
                            Parser.cil[Parser.nroDeInstrCorriente].accionInstr = Parser.AccionInstr.write;
                            Parser.cil[Parser.nroDeInstrCorriente].nro = item.val; // item.val;           //item.val;  aaaaaaa 

                            Code.cargaInstr("call write#(int32,int32)");
                            /////////////// Agrega ')' al arbol y lo muestra
                            Code.seleccLaProdEnLaGram(18);
                            Check(Token.RPAR);
                            write.Nodes.Add("')'");
                            MessageBoxCon3Preg(write);
                            Code.Colorear("token");
                            
                            ///////////////
                        }

                        MessageBoxCon3Preg();
                        ////////////// Agrega ';' al arbol y lo muestra
                        Check(Token.SEMICOLON);
                        write.Nodes.Add("';'");
                        MessageBoxCon3Preg(write);
                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        ///////////////
                        break;
                    case Token.LBRACE:
                        System.Windows.Forms.TreeNode blockint = new System.Windows.Forms.TreeNode("Block");
                        statement.Nodes.Add(blockint);
                        MessageBoxCon3Preg(statement);
                        Block(blockint);  //bloque(blockint = bloque interior)dentro de una sentencia
                        break;
                    case Token.SEMICOLON:
                        Check(Token.SEMICOLON);
                        statement.Nodes.Add("';'");
                        statement.ExpandAll();
                        MessageBoxCon3Preg(statement);
                        Code.Colorear("token");
                        MessageBoxCon3Preg();
                        break;
                    default:
                        Errors.Error("Espero una sentencia");
                        break;
                }
            }

            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(18);
            MessageBoxCon3Preg();
        } // Fin Statement

        /// G3 PERUBLOCK Arreglado el arbol de StatementsOpc cuando esta vacio (".")
        /// Y todos los nombres y padre en Block.
        static void Block(System.Windows.Forms.TreeNode methodDecl)
        {
            System.Windows.Forms.TreeNode block = new System.Windows.Forms.TreeNode("Block");
            
            methodDecl.Nodes.Add(block);
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(16);
            MessageBoxCon3Preg(block);
            ////// Agrega '{' al arbol
            Check(Token.LBRACE);
            block.Nodes.Add("'{'");
            block.ExpandAll();
            MessageBoxCon3Preg();
            Code.Colorear("token");
            MessageBoxCon3Preg();
            /////// Agrega 'StatementsOpc' al arbol
            System.Windows.Forms.TreeNode declarsAndSentences = new System.Windows.Forms.TreeNode("DeclarsAndSentences");
            block.Nodes.Add(declarsAndSentences);
            MessageBoxCon3Preg(block);
            Code.seleccLaProdEnLaGram(17);
            MessageBoxCon3Preg();
            /////// Agrega '.' al arbol si el block esta vacio
            if (la == Token.RBRACE)
            {
                Code.Colorear("latoken");
                MessageBoxCon3Preg();
                declarsAndSentences.Nodes.Add(".");
                declarsAndSentences.ExpandAll();
                MessageBoxCon3Preg(declarsAndSentences);
            }
            int ii = 1;
            while (la != Token.RBRACE)
            {
                if ((la == Token.IDENT || la == Token.IF || la == Token.WHILE || la == Token.BREAK
                  || la == Token.RETURN || la == Token.READ || la == Token.WRITE || la == Token.WRITELN
                  || la == Token.LBRACE || la == Token.SEMICOLON || la == Token.TYPE || la == Token.CONST) && la != Token.EOF)
                {
                    Code.Colorear("latoken");
                    MessageBoxCon3Preg();

                    if (la != Token.TYPE && la != Token.CONST) //Quiere decir que es una "sentencia"
                    {
                        System.Windows.Forms.TreeNode statement = new System.Windows.Forms.TreeNode("Statement");
                        declarsAndSentences.Nodes.Add(statement);
                        MessageBoxCon3Preg(statement);
                        Code.seleccLaProdEnLaGram(18);
                        MessageBoxCon3Preg();
                        if (ZZ.ParserStatem)
                        {
                            Console.WriteLine(".......Comienza statement nro:");
                            Console.Write(ii); Console.WriteLine("->" + laToken.str);
                        }

                        Statement(statement);  //dentro de block()

                        ii++;
                    }
                    else
                    {
                        //37.DeclarationInsideMethod = ConstDecl | VarDecl.

                        System.Windows.Forms.TreeNode DeclarationInsideMethod = new System.Windows.Forms.TreeNode("DeclarationInsideMethod");
                        declarsAndSentences.Nodes.Add(DeclarationInsideMethod);
                        MessageBoxCon3Preg(declarsAndSentences);
                        Code.seleccLaProdEnLaGram(37);
                        MessageBoxCon3Preg();
                        Code.Colorear("latoken");
                        MessageBoxCon3Preg();
                        //Code.CreateMetadata(curMethod);

                        if (la != Token.LBRACE && la != Token.EOF)
                        {
                            if (la == Token.TYPE)
                            {
                                //encuentraDecl = true;
                                System.Windows.Forms.TreeNode hijoVarDecl = new System.Windows.Forms.TreeNode("VarDecl");
                                DeclarationInsideMethod.Nodes.Add(hijoVarDecl);
                                MessageBoxCon3Preg();
                                Code.seleccLaProdEnLaGram(6);
                                MessageBoxCon3Preg();
                                VardDecl(Symbol.Kinds.Local, hijoVarDecl); // int x,i; en MethodDecl()  con int ya consumido

                                Code.cargaProgDeLaGram("IdentifiersOpc = .");
                            }
                            else if(la == Token.CONST)
                            {
                                System.Windows.Forms.TreeNode hijoConstDecl = new System.Windows.Forms.TreeNode("ConstDecl");
                                DeclarationInsideMethod.Nodes.Add(hijoConstDecl);
                                MessageBoxCon3Preg();
                                ConstDecl(hijoConstDecl);
                            }

                            Code.seleccLaProdEnLaGram(37);
                            MessageBoxCon3Preg();
                        }
                                
                        Code.Colorear("latoken");
                        MessageBoxCon3Preg();
                        Code.seleccLaProdEnLaGram(18);
                        MessageBoxCon3Preg();
                    }

                    //Haya sido Declaration o Statement debe volver a la regla 17
                    Code.seleccLaProdEnLaGram(17);
                    MessageBoxCon3Preg();

                }//Fin if 
                else
                {
                    //Si no se cumple esta condici�n es que llegamos al final "}", y debe salir del While
                    if (la != Token.RBRACE)
                    {
                        token.line = Scanner.line; token.col = Scanner.col - 1;
                        token.str = "?";
                        Errors.Error("Espero una sentencia o una declaraci�n");
                    }   
                }
                

            }//Fin while

            //Aca genera las instrucciones de las "Declaraciones de variables"
            if (cantVarLocales > 0)
            {
                string instrParaVarsLocs = ".locals init(int32 V_0";
                for (int i = 1; i < cantVarLocales; i++)
                {
                    instrParaVarsLocs = instrParaVarsLocs + "," + "\n          int32 V_" + i.ToString(); // +"  ";
                }
                instrParaVarsLocs = instrParaVarsLocs + ")";
                Code.cargaInstr(instrParaVarsLocs);

            }

            System.Windows.Forms.TreeNode DeclarsAndSentencesAux = new System.Windows.Forms.TreeNode("DeclarsAndSentences");
            declarsAndSentences.Nodes.Add(DeclarsAndSentencesAux);
            MessageBoxCon3Preg();
            DeclarsAndSentencesAux.Nodes.Add(".");
            DeclarsAndSentencesAux.ExpandAll();
            MessageBoxCon3Preg();
            

            Code.seleccLaProdEnLaGram(16);
            MessageBoxCon3Preg();
            block.Nodes.Add("'}'");
            MessageBoxCon3Preg();

            Check(Token.RBRACE);
            Code.Colorear("token");
            MessageBoxCon3Preg();
            
        }//Fin Block

        static void ActPars()
        {
            Item item;

            Code.seleccLaProdEnLaGram(23);
            MessageBoxCon3Preg();
            Expr(out item, null);
            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(19);
            MessageBoxCon3Preg();

            while (la == Token.COMMA && la != Token.EOF)
            {
                Code.seleccLaProdEnLaGram(20);
                MessageBoxCon3Preg();

                Check(Token.COMMA);
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(23);
                MessageBoxCon3Preg();
                Expr(out item, null);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(19);
                MessageBoxCon3Preg();
            }
        }//Fin ActPars

        static void Condition(out Item x)
        {
            Item y;
            CondTerm(out x);
            while (la == Token.OR && la != Token.EOF)
            {
                Check(Token.OR);
                Code.TJump(x);
                Code.il.MarkLabel(x.fLabel);
                CondTerm(out y);
                x.relop = y.relop; x.fLabel = y.fLabel;
            }
        }//Fin Condition

        static void Condition(System.Windows.Forms.TreeNode padre)
        {
            System.Windows.Forms.TreeNode hijo = new System.Windows.Forms.TreeNode("T�rmino");
            padre.Nodes.Add(hijo);
            CondTerm(hijo);
            while (la == Token.OR && la != Token.EOF)
            {
                Check(Token.OR);
                System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("T�rmino");
                padre.Nodes.Add(hijo1);
                MessageBoxCon3Preg(padre);
                CondTerm(hijo1);
            }
        }//Fin Condition

        static void CondTerm(out Item x)
        {
            Item y;
            CondFact(out x);
            while (la == Token.AND && la != Token.EOF)
            {
                Check(Token.AND);
                Code.FJump(x);
                CondFact(out y);
                x.relop = y.relop; x.tLabel = y.tLabel;
            }
        }//Fin CondTerm

        static void CondTerm(System.Windows.Forms.TreeNode padre)
        {
            System.Windows.Forms.TreeNode hijo = new System.Windows.Forms.TreeNode("Factor");
            padre.Nodes.Add(hijo);
            CondFact(hijo);
            while (la == Token.AND && la != Token.EOF)
            {
                Check(Token.AND);
                System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("Factor");
                padre.Nodes.Add(hijo1);
                MessageBoxCon3Preg(padre);
                CondFact(hijo1);
            }
        }//Fin CondTerm

        static void Expr(out Item item)
        {
            OpCode op; Item itemSig;

            if (la == Token.MINUS)
            {
                Check(Token.MINUS);
                Term(out item, null);  //
                if (item.type != Tab.intType) Errors.Error("Operando debe ser de tipo int");
                if (item.kind == Item.Kinds.Const) item.val = -item.val;
                else
                {
                    Code.Load(item); Code.il.Emit(Code.NEG);
                };
            }
            else
            {
                Code.cargaProgDeLaGram("OpcMinus = .");
                Code.cargaProgDeLaGram("Term = Factor  OpcMulopFactor.");
                Code.cargaProgDeLaGram("Factor = Designator  OpcRestOfMethCall.");
                Code.cargaProgDeLaGram("Designator = ident opcRestOfDesignator.");
                Code.Colorear("latoken");//1� parte de Term (y de Factor), por ej 123
                MessageBoxCon3Preg();
                Term(out item);
            }
            string opString = "";
            while ((la == Token.PLUS || la == Token.MINUS) && la != Token.EOF)
            {
                Code.cargaProgDeLaGram("OpcAddopTerm = Addop Term.");
                if (la == Token.PLUS)
                {
                    Scan(); op = Code.ADD; opString = "add       ";

                    Code.cargaProgDeLaGram("Addop = '+'.");
                    Code.cargaProgDeLaGram("Term = Factor OpcMulopFactor.");
                    Code.Colorear("token");
                    MessageBoxCon3Preg();
                }
                else if (la == Token.MINUS)
                {
                    Scan(); op = Code.SUB; opString = "sub       "; Code.cargaProgDeLaGram("Addop = '-'.");

                    Code.cargaProgDeLaGram("Addop = '-'.");
                    Code.cargaProgDeLaGram("Term = Factor OpcMulopFactor.");
                    Code.Colorear("token");
                    MessageBoxCon3Preg();
                }
                else op = Code.DUP;
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Code.Load(item);
                Term(out itemSig);
                Code.Load(itemSig);
                if (item.type != Tab.intType || itemSig.type != Tab.intType)
                    Errors.Error("Los operandos deben ser de tipo int");

                nroDeInstrCorriente++;
                Code.il.Emit(op);
                Code.cargaInstr(opString);
                if (op == Code.ADD)
                    cil[nroDeInstrCorriente].accionInstr = AccionInstr.add;
                else if (op == Code.SUB)
                    cil[nroDeInstrCorriente].accionInstr = AccionInstr.sub;
                else
                    System.Windows.Forms.MessageBox.Show("aun no implementado 343323");
            }//Fin While
        }//Fin Expr

        static void CondFact(out Item x)
        {
            Item y; int op;
            Expr(out x); Code.Load(x);
            Relop(out op);
            Expr(out y); Code.Load(y);
            if (!x.type.CompatibleWith(y.type))
                Errors.Error("type mismatch");
            else if (x.type.IsRefType() &&
                    op != Token.EQ && op != Token.NE)
                Errors.Error("only equality checks ...");
            x = new Item(op, x.type);
        }//Fin CondFact

        static void CondFact(System.Windows.Forms.TreeNode padre)
        {
            System.Windows.Forms.TreeNode hijo1 = new System.Windows.Forms.TreeNode("Expresi�n");
            padre.Nodes.Add(hijo1);
            Item item1, item2;
            Expr(out item1, hijo1);
            System.Windows.Forms.TreeNode hijo2 = new System.Windows.Forms.TreeNode("Condici�n");
            padre.Nodes.Add(hijo2);
            MessageBoxCon3Preg(padre);
            Relop(hijo2);
            System.Windows.Forms.TreeNode hijo3 = new System.Windows.Forms.TreeNode("Expresi�n");
            padre.Nodes.Add(hijo3);
            MessageBoxCon3Preg(padre);
            Expr(out item2, hijo3);
        }//Fin CondFact

        static void AA(out Item item)
        {
            item = new Item(12345);
        }//Fin AA

        //Inicio Modificaci�n - Grupo 1 - 28/10/15 - Se Arregl� Expr
        static void Expr(out Item item, System.Windows.Forms.TreeNode padre)
        {
            OpCode op; Item itemSig;
            System.Windows.Forms.TreeNode OpcMinus = new System.Windows.Forms.TreeNode("OpcMinus");
            System.Windows.Forms.TreeNode Term = new System.Windows.Forms.TreeNode("Term");
            if (la == Token.MINUS)
            {
                Check(Token.MINUS);
                padre.Nodes.Add(OpcMinus);
                MessageBoxCon3Preg();
                OpcMinus.Nodes.Add("-");
                OpcMinus.ExpandAll();
                MessageBoxCon3Preg(padre);
                Code.seleccLaProdEnLaGram(24);
                MessageBoxCon3Preg();
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(23);
                MessageBoxCon3Preg();

                padre.Nodes.Add(Term);
                MessageBoxCon3Preg(padre);
                Code.seleccLaProdEnLaGram(26);
                MessageBoxCon3Preg();
                Parser.Term(out item, Term);
                if (item.type != Tab.intType)
                    Errors.Error("Operando debe ser de tipo int");
                if (item.kind == Item.Kinds.Const)
                    item.val = -item.val;
                else
                    Code.Load(item); Code.il.Emit(Code.NEG);
            }
            else
            {
                Code.Colorear("latoken");
                MessageBoxCon3Preg();
                padre.Nodes.Add(OpcMinus);
                MessageBoxCon3Preg(padre);
                OpcMinus.Nodes.Add(".");
                OpcMinus.ExpandAll();
                MessageBoxCon3Preg(OpcMinus);
                padre.Nodes.Add(Term);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(26);
                MessageBoxCon3Preg();
                Parser.Term(out item, Term);
                
            }

            MessageBoxCon3Preg();
            Code.seleccLaProdEnLaGram(23);
            MessageBoxCon3Preg();

            string opString = "";
            System.Windows.Forms.TreeNode OpcAddopTerms = new System.Windows.Forms.TreeNode("OpcAddopTerms");
            padre.Nodes.Add(OpcAddopTerms);
            MessageBoxCon3Preg(padre);
            Code.seleccLaProdEnLaGram(25);
            MessageBoxCon3Preg();
            bool existe_Addop_opc = false;
            while ((la == Token.PLUS || la == Token.MINUS) && la != Token.EOF)
            {
                System.Windows.Forms.TreeNode AddOp = new System.Windows.Forms.TreeNode("AddOp");
                OpcAddopTerms.Nodes.Add(AddOp);
                MessageBoxCon3Preg(OpcAddopTerms);
                Code.seleccLaProdEnLaGram(33);
                MessageBoxCon3Preg();

                existe_Addop_opc = true;
                if (la == Token.PLUS)
                {
                    Scan();
                    op = Code.ADD;
                    opString = "add       ";
                    Code.Colorear("token");
                    MessageBoxCon3Preg();
                    AddOp.Nodes.Add("'+'");
                    AddOp.ExpandAll();
                    MessageBoxCon3Preg(AddOp);
                }
                else if (la == Token.MINUS)
                {
                    Scan();
                    op = Code.SUB;
                    opString = "sub       ";
                    Code.cargaProgDeLaGram("AddOp = '-'.");
                    Code.Colorear("token");
                    MessageBoxCon3Preg();
                    AddOp.Nodes.Add("'-'");
                    AddOp.ExpandAll();
                    MessageBoxCon3Preg(AddOp);
                }
                else op = Code.DUP;
                Code.Colorear("token");
                MessageBoxCon3Preg();
                Code.Load(item);
                System.Windows.Forms.TreeNode Term_OpcAddop = new System.Windows.Forms.TreeNode("Term");
                OpcAddopTerms.Nodes.Add(Term_OpcAddop);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(26);
                MessageBoxCon3Preg();

                Parser.Term(out itemSig, Term_OpcAddop);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(25);
                MessageBoxCon3Preg();

                Code.Load(itemSig);
                if (item.type != Tab.intType || itemSig.type != Tab.intType)
                    Errors.Error("Los operandos deben ser de tipo int");
                nroDeInstrCorriente++;
                Code.il.Emit(op);
                Code.cargaInstr(opString);
                if (op == Code.ADD)
                    cil[nroDeInstrCorriente].accionInstr = AccionInstr.add;
                else if (op == Code.SUB)
                    cil[nroDeInstrCorriente].accionInstr = AccionInstr.sub;
                else
                    System.Windows.Forms.MessageBox.Show("Aun no implementado 343323");
            }//Fin While

            if (existe_Addop_opc == false)
            {
                OpcAddopTerms.Nodes.Add(" . ");
                OpcAddopTerms.ExpandAll();
                MessageBoxCon3Preg(OpcAddopTerms);
            }

            Code.seleccLaProdEnLaGram(23);
            MessageBoxCon3Preg();

        }//Fin Expr
        //Fin Modificaci�n - Grupo 1 - 28/10/15

        //Inicio Modificaci�n - Grupo 1 - 28/10/15 - Se Arregl� Designator
        static void Designator(out Item item)
        {
            Check(Token.IDENT);
            Code.seleccLaProdEnLaGram(31);
            Code.cargaProgDeLaGram("Designator = ident  opcRestOfDesignator.");
            Symbol sym = Tab.Find(token.str);
            if (ZZ.ParserStatem)
                Console.WriteLine("token.str:" + token.str);
            if (sym == null)
                Errors.Error(sym.name + "..no est� en la Tab");
            item = new Item(sym);
            if ((la == Token.PERIOD || la == Token.LBRACK) && la != Token.EOF)
            {
                while ((la == Token.PERIOD || la == Token.LBRACK) && la != Token.EOF)
                {
                    if (ZZ.parser) Console.Write("field..." + token.str + " (val)");
                    if (la == Token.PERIOD)
                    {
                        Check(Token.PERIOD);
                        Code.cargaProgDeLaGram("opcRestOfDesignator =  '.' ident.");
                        Code.Colorear("token");
                   
                        if (muestraProducciones)
                            MessageBoxCon3Preg();
                        Check(Token.IDENT);
                        Code.Colorear("token");
                        
                        if (muestraProducciones)
                            MessageBoxCon3Preg();
                        if (ZZ.parser)
                            Console.WriteLine(" . " + token.str + " (pos)");
                        if (item.type.kind == Struct.Kinds.Class)
                        {
                            Code.Load(item);
                            Symbol symField = Tab.FindSymbol(token.str, sym.type.fields);
                            Struct xTypeField;
                            if (symField == null)
                            {
                                Errors.Error("..--debe venir un tipo");
                                xTypeField = Tab.noType;
                            }
                            else
                            {
                                if (ZZ.parser) Console.WriteLine("encuentra " + symField.name);
                                xTypeField = symField.type; //Devuelve int como tipo (Struct), no como nodo Symbol 
                            };
                            item.sym = symField;
                            item.type = item.sym.type;
                        }
                        else Errors.Error(sym.name + " is not an object");
                        item.kind = Item.Kinds.Field;
                    }
                    else
                        if (la == Token.LBRACK)
                        {
                            Check(Token.LBRACK);
                            Code.cargaProgDeLaGram("opcRestOfDesignator =  '[' Expr ']'.");
                            Code.Colorear("token");
                            if (muestraProducciones) MessageBoxCon3Preg();

                            Code.Load(item);
                            Item itemSig;
                            Expr(out itemSig);

                            if (item.type.kind == Struct.Kinds.Arr)
                            {
                                if (itemSig.type != Tab.intType) Errors.Error("index must be of type int");
                                Code.Load(itemSig);
                                itemSig.type = item.type.elemType;
                            }
                            else Errors.Error(sym.name + " is not an array");
                            item.kind = Item.Kinds.Elem;
                            Check(Token.RBRACK);
                        }
                }//Fin while
            }
            else
            {
                Code.cargaProgDeLaGram("opcRestOfDesignator =  .");
                Code.Colorear("latoken");
                if (muestraProducciones)
                    MessageBoxCon3Preg();
                Code.Load(item);
            }
        }//Fin Designator
        //Fin Modificaci�n - Grupo 1 - 28/10/15

        static void Factor(out Item item)
        {
            Struct xType;
            if (la == Token.IDENT)
            {

                Designator(out item); //en el Factor
                if (la == Token.LPAR)
                {
                    Check(Token.LPAR);
                    Code.cargaProgDeLaGram("OpcRestOfMethCall = '(' OpcActPars ')'.");
                    Code.Colorear("token"); // el "("
                    if (muestraProducciones) MessageBoxCon3Preg();
                    if (la == Token.MINUS || la == Token.IDENT ||
                        la == Token.NUMBER || la == Token.CHARCONST ||
                        la == Token.NEW || la == Token.LPAR)
                        ActPars();
                    Check(Token.RPAR);
                }
                else
                {
                    Code.cargaProgDeLaGram("OpcRestOfMethCall = .");
                    Code.Colorear("latoken"); // el "("
                    if (muestraProducciones) MessageBoxCon3Preg();
                }
            }
            else
                switch (la)
                {
                    case Token.NUMBER:
                        {
                            Check(Token.NUMBER);
                            Code.cargaProgDeLaGram("Factor = number.");
                            Code.Colorear("token");
                                MessageBoxCon3Preg();
                            item = new Item(token.val);//Nuevo
                            Code.Load(item);
                            break;
                        }
                    case Token.CHARCONST:
                        {
                            Check(Token.CHARCONST);
                            item = new Item(token.val); item.type = Tab.charType;
                            break;
                        }
                    case Token.NEW:
                        {
                            Check(Token.NEW);
                            Check(Token.IDENT);  //Deberia buscar en la Tab y verificar que sea un Tipo o una clase 
                            Symbol sym = Tab.Find(token.str);  //ident debe ser int, char, o una clase  (Table)
                            if (sym.kind != Symbol.Kinds.Type) Errors.Error("type expected");
                            Struct type = sym.type;
                            //si es clase, sym.type contiene un puntero a los campos de esa clase
                            if (ZZ.parser)
                                Console.WriteLine("Tab.Find(" + token.str + ") =>" + sym.ToString() + "...");
                            if (sym == null)
                            {
                                Errors.Error("--debe venir un tipo");
                                xType = Tab.noType;
                            }
                            else
                            {
                                xType = sym.type; //Devuelve int como tipo (Struct), no como nodo Symbol 
                                if (ZZ.parser) Console.WriteLine("Encontr� " + token.str);
                            };
                            if (ZZ.parser) Console.WriteLine("Termin� new " + token.str);

                            if (la == Token.LBRACK)
                            {
                                Check(Token.LBRACK);
                                Expr(out item);
                                if (item.type != Tab.intType) Errors.Error("array size must be of type int");
                                Code.Load(item); //genera cod p/cargar el result de la expr	
                                Code.il.Emit(Code.NEWARR, type.sysType); //NEWARR de char
                                type = new Struct(Struct.Kinds.Arr, type);
                                //el nuevo type ser� array de char (pag 33 de T de simb)
                                Check(Token.RBRACK);
                            }
                            else
                            {
                                if (sym.ctor == null)
                                {
                                    Console.WriteLine("Error sym.ctor == null"); if (ZZ.readKey) Console.ReadKey();
                                };
                                if (type.kind == Struct.Kinds.Class) //new Table  pag 34 de T. De Simb	  
                                    Code.il.Emit(Code.NEWOBJ, sym.ctor); //emite cod p/new Table  qq1

                                else { Errors.Error("class type expected"); type = Tab.noType; }
                            }
                            item = new Item(type);
                            break;
                        }
                    case Token.LPAR:
                        {
                            Check(Token.LPAR);
                            Expr(out item);
                            Check(Token.RPAR);
                            break;
                        }
                    default:
                        {
                            Errors.Error(ErrorStrings.INVALID_FACT);
                            item = new Item(3);
                            break;
                        }
                }
        }//Fin Factor

        static void Term(out Item item)
        {
            OpCode op; Item itemSig; string opString = "";
            if (la == Token.IDENT || la == Token.NUMBER || la == Token.CHARCONST || la == Token.NEW || la == Token.LPAR)
            {
                Factor(out item);
                while ((la == Token.TIMES || la == Token.SLASH || la == Token.REM) && la != Token.EOF)
                {
                    Code.cargaProgDeLaGram("OpcMulopFactor = Mulop Factor.");
                    switch (la)
                    {
                        case Token.TIMES:
                            {
                                Check(Token.TIMES); op = Code.MUL; opString = "mul       ";
                                Code.Colorear("token");
                                Code.cargaProgDeLaGram("Mulop =	'*'.");
                                if (muestraProducciones) MessageBoxCon3Preg();
                                break;
                            }
                        case Token.SLASH:
                            {
                                Check(Token.SLASH); op = Code.DIV; opString = "div       ";
                                Code.Colorear("token");
                                Code.cargaProgDeLaGram("Mulop =	'/'.");
                                if (muestraProducciones) MessageBoxCon3Preg();
                                break;
                            }
                        case Token.REM:
                            {
                                Check(Token.REM); op = Code.REM;
                                System.Windows.Forms.MessageBox.Show("aun no implementado");
                                break;
                            }
                        default:
                            {
                                Errors.Error(ErrorStrings.MUL_OP);
                                op = Code.REM;
                                break;
                            }
                    } //Fin switch
                    Code.Load(item);
                    Factor(out itemSig);
                    Code.Load(itemSig);
                    if (item.type != Tab.intType || itemSig.type != Tab.intType)
                        Errors.Error("Debe venir un Term");
                    Code.il.Emit(op);
                    nroDeInstrCorriente++;
                    Code.cargaInstr(opString);
                    if (op == Code.MUL)
                        cil[nroDeInstrCorriente].accionInstr = AccionInstr.mul;
                    else if (op == Code.DIV)
                        cil[nroDeInstrCorriente].accionInstr = AccionInstr.div;
                    else
                        System.Windows.Forms.MessageBox.Show("aun no implementado 343323");
                }//Fin while
                Code.cargaProgDeLaGram("OpcMulopFactor = .");
                Code.Colorear("latoken");
                    MessageBoxCon3Preg();
            }
            else
            {
                Errors.Error("ErrorStrings.MUL_OP");
                item = new Item(0);
            }
        }//Fin Term

        //Inicio Modificac�on - Grupo 1 - 28/10/15 - Se Arregl� Term
        static void Term(out Item item, System.Windows.Forms.TreeNode padre)
        {
            OpCode op; Item itemSig; string opString = "";
            if (la == Token.IDENT || la == Token.NUMBER || la == Token.CHARCONST || la == Token.NEW || la == Token.LPAR)
            {
                
                System.Windows.Forms.TreeNode Factor = new System.Windows.Forms.TreeNode("Factor");
                padre.Nodes.Add(Factor);
                MessageBoxCon3Preg(padre);
                Code.seleccLaProdEnLaGram(28);
                MessageBoxCon3Preg();
                Parser.Factor(out item, Factor);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(26);
                MessageBoxCon3Preg();
                bool existe_OpcMulOpFactor = false;
                System.Windows.Forms.TreeNode OpcMulopFactors = new System.Windows.Forms.TreeNode("OpcMulopFactors");
                padre.Nodes.Add(OpcMulopFactors);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(27);
                MessageBoxCon3Preg();

                while ((la == Token.TIMES || la == Token.SLASH || la == Token.REM) && la != Token.EOF)
                {
                    Code.cargaProgDeLaGram("OpcMulopFactor = Mulop Factor.");
                    System.Windows.Forms.TreeNode MulOp = new System.Windows.Forms.TreeNode("MulOp");
                    OpcMulopFactors.Nodes.Add(MulOp);
                    MessageBoxCon3Preg(OpcMulopFactors);
                    switch (la)
                    {
                        case Token.TIMES:
                            {
                                Check(Token.TIMES);
                                op = Code.MUL;
                                opString = "mul       ";
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                                Code.cargaProgDeLaGram("Mulop =	'*'.");
                                existe_OpcMulOpFactor = true;
                                Code.seleccLaProdEnLaGram(34);
                                MessageBoxCon3Preg();
                                MulOp.Nodes.Add("'*'");
                                MulOp.ExpandAll();
                                MessageBoxCon3Preg(MulOp);
                                break;
                            }
                        case Token.SLASH:
                            {
                                Check(Token.SLASH);
                                op = Code.DIV;
                                opString = "div       ";
                                existe_OpcMulOpFactor = true;
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                                Code.cargaProgDeLaGram("Mulop =	'/'.");
                                Code.seleccLaProdEnLaGram(34);
                                MessageBoxCon3Preg();
                                MulOp.Nodes.Add("'/'");
                                MulOp.ExpandAll();
                                MessageBoxCon3Preg(MulOp);
                                break;
                            }
                        case Token.REM:
                            Check(Token.REM);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            op = Code.REM;
                            System.Windows.Forms.MessageBox.Show("aun no implementado");
                            break;
                        default:
                            Errors.Error(ErrorStrings.MUL_OP);
                            op = Code.REM;
                            break;
                    } //Fin switch
                    Code.Load(item);
                    System.Windows.Forms.TreeNode Factor_OpcMulop = new System.Windows.Forms.TreeNode("Factor");
                    OpcMulopFactors.Nodes.Add(Factor_OpcMulop);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(27);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(28);
                    MessageBoxCon3Preg();
                    Parser.Factor(out itemSig, Factor_OpcMulop);
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(27);
                    MessageBoxCon3Preg();
                    Code.Load(itemSig);
                    if (item.type != Tab.intType || itemSig.type != Tab.intType)
                    {
                        Errors.Error("Debe venir un Term");
                    }
                    Code.il.Emit(op);
                    nroDeInstrCorriente++;
                    Code.cargaInstr(opString);
                    if (op == Code.MUL)
                        cil[nroDeInstrCorriente].accionInstr = AccionInstr.mul;
                    else if (op == Code.DIV)
                        cil[nroDeInstrCorriente].accionInstr = AccionInstr.div;
                    else
                        System.Windows.Forms.MessageBox.Show("aun no implementado 343323");
                }//Fin while
                if (existe_OpcMulOpFactor == false)
                {
                    OpcMulopFactors.Nodes.Add(".");
                    OpcMulopFactors.ExpandAll();
                    MessageBoxCon3Preg(OpcMulopFactors);
                    Code.cargaProgDeLaGram("OpcMulopFactor = .");
                    Code.Colorear("latoken");
                    MessageBoxCon3Preg();
                }
 
            }
            else
            {
                Errors.Error("ErrorStrings.MUL_OP");
                item = new Item(0);
            }

            Code.seleccLaProdEnLaGram(26);
            MessageBoxCon3Preg();
        }//Fin Term
        //Fin Modificac�on - Grupo 1 - 28/10/15

        //Inicio Modificac�on - Grupo 1 - 28/10/15 - Se Arregl� Factor
        static void Factor(out Item item, System.Windows.Forms.TreeNode padre)
        {
            Struct xType;
            if (la == Token.IDENT)
            {
                System.Windows.Forms.TreeNode Designator = new System.Windows.Forms.TreeNode("Designator");
                padre.Nodes.Add(Designator);
                MessageBoxCon3Preg(padre);
                Parser.Designator(out item, Designator); //en el Factor
                MessageBoxCon3Preg();

                System.Windows.Forms.TreeNode opcRestOfMethCall = new System.Windows.Forms.TreeNode("OpcRestOfMethCall");
                padre.Nodes.Add(opcRestOfMethCall);
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(29);
                MessageBoxCon3Preg();
                if (la == Token.LPAR)
                {
                    Check(Token.LPAR);
                    Code.cargaProgDeLaGram("OpcRestOfMethCall = '(' OpcActPars ')'.");
                    Code.Colorear("token"); // el "("
  
                    MessageBoxCon3Preg();
                    Code.seleccLaProdEnLaGram(30);
                    MessageBoxCon3Preg();

                    if (la == Token.MINUS || la == Token.IDENT || la == Token.NUMBER || la == Token.CHARCONST || la == Token.NEW || la == Token.LPAR)
                    {
                        Code.seleccLaProdEnLaGram(19);
                        MessageBoxCon3Preg();
                        ActPars();  //Esta parte falta
                    }
                    Check(Token.RPAR);
                }
                else
                {
                    opcRestOfMethCall.Nodes.Add(".");
                    opcRestOfMethCall.ExpandAll();
                    MessageBoxCon3Preg();
                    Code.cargaProgDeLaGram("OpcRestOfMethCall = .");
                    Code.Colorear("latoken"); // el "("
                    MessageBoxCon3Preg();
                }
                MessageBoxCon3Preg();
                Code.seleccLaProdEnLaGram(29);
                MessageBoxCon3Preg();
            }
            else
                switch (la)
                {
                    case Token.NUMBER:
                        {
                            Check(Token.NUMBER);
                            Code.cargaProgDeLaGram("Factor = number.");
                            Code.Colorear("token");
                            System.Windows.Forms.TreeNode number = new System.Windows.Forms.TreeNode("number.");
                            padre.Nodes.Add(number);
                            MessageBoxCon3Preg(padre);
                            item = new Item(token.val);//Nuevo
                            Code.Load(item);
                            break;
                        }
                    case Token.CHARCONST:
                        {
                            Check(Token.CHARCONST);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            item = new Item(token.val); item.type = Tab.charType;
                            break;
                        }
                    case Token.NEW:
                        {
                            Check(Token.NEW);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            Check(Token.IDENT);  //Deberia buscar en la Tab y verificar que sea un Tipo o una clase 
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            Symbol sym = Tab.Find(token.str);  //ident debe ser int, char, o una clase  (Table)
                            if (sym.kind != Symbol.Kinds.Type)
                                Errors.Error("type expected");
                            Struct type = sym.type;
                            //si es clase, sym.type contiene un puntero a los campos de esa clase
                            if (ZZ.parser)
                                Console.WriteLine("Tab.Find(" + token.str + ") =>" + sym.ToString() + "...");
                            if (sym == null)
                            {
                                Errors.Error("--debe venir un tipo");
                                xType = Tab.noType;
                            }
                            else
                            {
                                xType = sym.type; //Devuelve int como tipo (Struct), no como nodo Symbol 
                                if (ZZ.parser)
                                    Console.WriteLine("Encontr� " + token.str);
                            };
                            if (ZZ.parser)
                                Console.WriteLine("Termin� new " + token.str);
                            if (la == Token.LBRACK)
                            {
                                Check(Token.LBRACK);
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                                Parser.Expr(out item);
                                if (item.type != Tab.intType) Errors.Error("array size must be of type int");
                                Code.Load(item); //genera cod p/cargar el result de la expr	
                                Code.il.Emit(Code.NEWARR, type.sysType); //NEWARR de char
                                type = new Struct(Struct.Kinds.Arr, type);
                                Check(Token.RBRACK);
                                Code.Colorear("token");
                                MessageBoxCon3Preg();
                            }
                            else
                            {
                                if (sym.ctor == null)
                                {
                                    Console.WriteLine("Error sym.ctor == null"); if (ZZ.readKey) Console.ReadKey();
                                };
                                if (type.kind == Struct.Kinds.Class) //new Table  pag 34 de T. De Simb
                                {
                                    Code.il.Emit(Code.NEWOBJ, sym.ctor); //emite cod p/new Table  qq1
                                }
                                else
                                {
                                    Errors.Error("class type expected"); type = Tab.noType;
                                }
                            }
                            item = new Item(type);
                            //item.type = type;  lo hace en el constr Item(Struct type)
                            break;
                        }
                    case Token.LPAR:
                        {
                            Check(Token.LPAR);
                            padre.Nodes.Add("'('");
                            padre.ExpandAll();
                            MessageBoxCon3Preg(padre);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();

                            System.Windows.Forms.TreeNode Expr = new System.Windows.Forms.TreeNode("Expr");
                            padre.Nodes.Add(Expr);
                            MessageBoxCon3Preg(padre);
                            Parser.Expr(out item, Expr);
                            Check(Token.RPAR);
                            padre.Nodes.Add("')'");
                            MessageBoxCon3Preg(padre);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            break;
                        }
                    default:
                        {
                            Errors.Error(ErrorStrings.INVALID_FACT);
                            item = new Item(3);
                            break;
                        }
                }
        }//Fin Factor
        //Fin Modificaci�n - Grupo 1 - 28/10/15

        static void Designator(out Item item, System.Windows.Forms.TreeNode padre)
        {
            
                //debe buscar el designator en la tabla de simbolos
                Check(Token.IDENT); //ahora token.str="val"      y laToken= "="
                                    //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
                padre.Nodes.Add("ident");
                padre.ExpandAll();
                MessageBoxCon3Preg(padre);
                Code.seleccLaProdEnLaGram(31);
                MessageBoxCon3Preg();
            //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
            System.Windows.Forms.TreeNode hijo2 = new System.Windows.Forms.TreeNode("opcRestOfDesignator");
                padre.Nodes.Add(hijo2);
                MessageBoxCon3Preg(padre);
                Code.seleccLaProdEnLaGram(32);
            MessageBoxCon3Preg();
            //-------------------------------------------------Grupo 2 28/9/2015-----------------------------------------------------------
            Symbol sym = Tab.Find(token.str);
                if (ZZ.ParserStatem) Console.WriteLine("token.str:" + token.str);
                if (sym == null) Errors.Error(sym.name + "..no est� en la Tab");

                item = new Item(sym);
                if ((la == Token.PERIOD || la == Token.LBRACK) && la != Token.EOF)
                {
                    while ((la == Token.PERIOD || la == Token.LBRACK) && la != Token.EOF)//hacer do...while
                    {
                        //debe seguir buscando en la Tab
                        if (ZZ.parser) Console.Write("field..." + token.str + " (val)"); //val
                        if (la == Token.PERIOD)
                        {
                            Check(Token.PERIOD); //caso del val . pos
                            Code.cargaProgDeLaGram("opcRestOfDesignator =  '.' ident.");
                            Code.Colorear("token");
                        MessageBoxCon3Preg();
                        System.Windows.Forms.TreeNode hijo3 = new System.Windows.Forms.TreeNode("opcRestOfDesignator =  '.' ident.");
                            hijo2.Nodes.Add(hijo3);
                            MessageBoxCon3Preg(hijo2);
                            hijo3.Nodes.Add("'.'");
                            hijo3.ExpandAll();
                            MessageBoxCon3Preg(hijo3);
                            Check(Token.IDENT); //pos
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            hijo3.Nodes.Add("'Ident'");
                            MessageBoxCon3Preg(hijo3);

                            if (ZZ.parser) Console.WriteLine(" . " + token.str + " (pos)"); //pos
                            if (item.type.kind == Struct.Kinds.Class)
                            {
                                Code.Load(item);  //lleva el Item al  Stack
                                Symbol symField = Tab.FindSymbol(token.str, sym.type.fields);
                                //sim = Tab.FindSymbol(token.str, sym.type.fields);//pierde sim orig pero sirve,
                                // para luego hacer item = new Item(sym);
                                Struct xTypeField;
                                if (symField == null)
                                {
                                    Errors.Error("..--debe venir un tipo");//Fran
                                    xTypeField = Tab.noType;
                                }
                                else
                                {
                                    if (ZZ.parser) Console.WriteLine("encuentra " + symField.name);
                                    xTypeField = symField.type; //Devuelve int como tipo (Struct), no como nodo Symbol 
                                };
                                item.sym = symField; // Tab.FindField(token.str, item.type);
                                item.type = item.sym.type; //int        f     clase
                            }
                            else Errors.Error(sym.name + " is not an object");
                            item.kind = Item.Kinds.Field; //Field
                        }
                        else
                            if (la == Token.LBRACK)
                        {
                            Check(Token.LBRACK);
                            Code.cargaProgDeLaGram("opcRestOfDesignator =  '[' Expr ']'.");
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                            if (muestraProducciones) MessageBoxCon3Preg();

                            Code.Load(item);
                            Item itemSig;
                            Expr(out itemSig, null);

                            if (item.type.kind == Struct.Kinds.Arr)
                            {
                                if (itemSig.type != Tab.intType) Errors.Error("index must be of type int");
                                Code.Load(itemSig);  //carga el subindice en el Stack
                                itemSig.type = item.type.elemType;//Si char[10] a; => x.type quedar� con char
                            }
                            else Errors.Error(sym.name + " is not an array");
                            item.kind = Item.Kinds.Elem;

                            Check(Token.RBRACK);
                            Code.Colorear("token");
                            MessageBoxCon3Preg();
                    }
                    }//Fin while
                }
                else
                {
                    Code.cargaProgDeLaGram("opcRestOfDesignator =  .");
                    //-------------------------------------------------Grupo 2 30/9/2015-----------------------------------------------------------
                    hijo2.Nodes.Add(".");
                    hijo2.ExpandAll();
                    MessageBoxCon3Preg(hijo2);
                    Code.Load(item);
                }
                Code.seleccLaProdEnLaGram(31);
                MessageBoxCon3Preg();
            
        } //Fin Designator

        static void Relop(out int op)
        {
            switch (la)
            {
                case Token.EQ:
                    Check(Token.EQ); op = Token.EQ;
                    break;
                case Token.NE:
                    Check(Token.NE); op = Token.NE;
                    break;
                case Token.GT:
                    Check(Token.GT); op = Token.GT;
                    break;
                case Token.GE:
                    Check(Token.GE); op = Token.GE;
                    break;
                case Token.LT:
                    Check(Token.LT); op = Token.LT;
                    break;
                case Token.LE:
                    Check(Token.LE); op = Token.LE;
                    break;
                default:
                    Errors.Error(ErrorStrings.REL_OP); op = Token.EQ; //Solo para q no de error
                    break;
            }
        }//Fin Relop

        static void Relop(System.Windows.Forms.TreeNode padre)
        {
            switch (la)
            {
                case Token.EQ:
                    Check(Token.EQ);
                    break;
                case Token.NE:
                    Check(Token.NE);
                    break;
                case Token.GT:
                    Check(Token.GT);
                    break;
                case Token.GE:
                    Check(Token.GE);
                    break;
                case Token.LT:
                    Check(Token.LT);
                    break;
                case Token.LE:
                    Check(Token.LE);
                    break;
                default:
                    Errors.Error(ErrorStrings.REL_OP);
                    break;
            }
            padre.Nodes.Add(la.ToString());
        }//Fin Relop

        static void Addop()
        {
            if (la == Token.PLUS)
                Check(Token.PLUS);
            else
                if (la == Token.MINUS)
                    Check(Token.MINUS);
                else
                    Errors.Error(ErrorStrings.ADD_OP);
        }//Fin Addop

        static void Mulop()
        {
            if (la == Token.TIMES)
                Check(Token.TIMES);
            else
                if (la == Token.SLASH)
                    Check(Token.SLASH);
                else
                    if (la == Token.REM)
                        Check(Token.REM);
                    else
                        Errors.Error(ErrorStrings.MUL_OP);
        }//Fin Mulop

        // M�todos agregados por Manuel para manejo de errores
        static BitArray addConjuntos(BitArray a, BitArray b)
        {
            BitArray c = (BitArray)a.Clone();
            c.Or(b);
            return c;
        }
        static void testParsing(BitArray c1, BitArray c2, int nro_error)
        {
            if (c1[la] == false) // lookahead no est� en c1
            {
                Errors.Error(ErrorStrings.ADD_OP);
                c1 = addConjuntos(c1, c2);
                while (c1[la] == false)
                    Scan();
            }
        }
        /* Starts the analysis. 
         * Output is written to System.out.
         */

        public static void Parse(string prog)
        {
            Scanner.Init(new StringReader(prog), null);  //deja en ch el 1� char de prog 
            Tab.Init();  //topScope queda apuntando al Scope p/el universe
            Errors.Init();
            curMethod = null;
            token = null;
            //Con 1� char de prog en la vble ch, ya puede comenzar el Scanner 
            laToken = new Token(1, 1);  // avoid crash when 1st symbol has scanner error
            //porque el Scan comienza con token = laToken
            Scan();                     // scan first symbol
            Program();                  // start analysis  
            Check(Token.EOF);
        }

        /* Handles all compiler errors. */
        public class Errors
        {
            /* Minimal number of tokens between errors.
             * Errors are only reported if error distance is greater than this. */
            const int minDist = 3;
            /* Current distance from last syntax error. */
            public static int dist;
            /* Error count. */
            static int cnt;
            /* Print error message to output and count reported errors. */
            public static void Error(string msg)
            {
                /*---------------------------------*/
                /*----- insert your code here -----*/
                /*---------------------------------*/
                cnt++;
                //Console.WriteLine("Error: line {0}, col {1}: {2}", token.line, token.col, msg);
                int linea = token.line; int col = token.col;
                int sizeToken;
                if (token.str == null) sizeToken = 1;
                else sizeToken = token.str.Length;   //aqui
                if (sizeToken < 1) sizeToken = 1;
                throw new ErrorMio(linea, col, sizeToken, msg);
                //richTextBox1.Select(richTextBox1.GetFirstCharIndexFromLine(linea) + col,
                //                    sizeToken);
                //richTextBox1.SelectionColor = Color.Red;
                //Console.WriteLine(" toque una tecla para continuar");
                //if (ZZ.readKey) Console.ReadKey();
            }//Fin class Error

            public static int Count { get { return cnt; } }

            public static void Init() { cnt = 0; Reset(); }

            public static void Reset()
            {
                dist = minDist;  // don't skip errors any more
            }
        }
    }//Fin class Parser
}//Fin namespace