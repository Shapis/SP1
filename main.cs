using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class main : Node2D
{
    [Export] Sprite2D sprite2D;
    [Export] Sprite2D quadradoDebug;
    [Export] Sprite2D tiro;
    //[Export] Line2D line2D;
    [Export] Timer timer;
    [Export] RichTextLabel[] textos1;
    [Export] RichTextLabel[] textos2;
    [Export] int numPontos = 1000;

    Escritor escritor = new();

    private Vector2 posicaoInicial = new();
    private int coluna = 0;
    private int linha = 0;
    private int matriz = 0;
    private int wait = 0;
    private RandomNumberGenerator rng = new();
    int[,] tabela = new int[8, 8];
    int[] diametro = new int[2];
    int numFig = 0;
    String figRef = "";
    String stringCsv = "";


    int cnt = 0;
    int numPixelsInternos = 0;
    int numPixelsBorda = 0;
    float perimetro = 0;

    bool espera_um_frame = true;


    public override void _Ready()
    {
        //sprite2D.Texture = GD.Load<Texture2D>("res://dados/Figuras1213.png");
        quadradoDebug.Position = new Vector2(Estaticos.ORIGEMX + (Estaticos.TAMANHO / 2), Estaticos.ORIGEMY + (Estaticos.TAMANHO / 2));
        posicaoInicial = quadradoDebug.Position;
        timer.Timeout += HandleTimeout;
        //Daltonista(quadradoDebug.Position - new Vector2((Estaticos.TAMANHO / 2), (Estaticos.TAMANHO / 2)) + new Vector2((Estaticos.TAMANHO + Estaticos.DISTANCIA), 0), 0);
        //GetTree().Quit();
    }


    private void HandleTimeout()
    {
        if (espera_um_frame)
        {
            espera_um_frame = false;
            return;
        }

        Auxiliar auxiliar = new();
        //GD.Print('(', linha, ' ', coluna, ')');
        if (coluna < 7)
        {
            //GD.Print(quadradoDebug.Position);
            Daltonista(quadradoDebug.Position - new Vector2((Estaticos.TAMANHO / 2), (Estaticos.TAMANHO / 2)), matriz);
            if (matriz == 0)
            {
                quadradoDebug.Position += new Vector2((Estaticos.TAMANHO + Estaticos.DISTANCIA), 0);
            }
            else
            {
                quadradoDebug.Position += new Vector2((Estaticos.TAMANHO + Estaticos.DISTANCIA2), 0);
            }
            coluna += 1;
        }
        else if (linha < 7)
        {
            Daltonista(quadradoDebug.Position - new Vector2((Estaticos.TAMANHO / 2), (Estaticos.TAMANHO / 2)), matriz);
            coluna = 0;
            linha += 1;
            if (matriz == 0)
            {
                quadradoDebug.Position += new Vector2((Estaticos.TAMANHO + Estaticos.DISTANCIA) * -7, (Estaticos.TAMANHO + Estaticos.DISTANCIA));
            }
            else
            {
                quadradoDebug.Position += new Vector2((Estaticos.TAMANHO + Estaticos.DISTANCIA2) * -7, (Estaticos.TAMANHO + Estaticos.DISTANCIA2));
            }
        }
        else if (matriz == 0)
        {
            Daltonista(quadradoDebug.Position - new Vector2((Estaticos.TAMANHO / 2), (Estaticos.TAMANHO / 2)), matriz);
            //GD.Print("Terminou a leitura 1!");
            linha = 0;
            coluna = 0;
            matriz = 1;
            quadradoDebug.Position = new Vector2(Estaticos.ORIGEMX2 + (Estaticos.TAMANHO / 2), Estaticos.ORIGEMY2 + (Estaticos.TAMANHO / 2));
            cnt = 0;
            numPixelsInternos = 0;
            numPixelsBorda = 0;
            perimetro = 0;
            var convexos = PixeisConvexos(tabela);

            textos1[2].Text = convexos.Item1.ToString();
            textos1[3].Text = convexos.Item2.ToString();

            (diametro[0], diametro[1]) = auxiliar.MedeDiametro(tabela);
            textos1[5].Text = diametro[0].ToString();
            textos1[6].Text = diametro[1].ToString();

            tabela = new int[8, 8];
        }
        else if (matriz == 1)
        {
            Daltonista(quadradoDebug.Position - new Vector2((Estaticos.TAMANHO / 2), (Estaticos.TAMANHO / 2)), matriz);
            //GD.Print("Terminou a leitura 2!");
            matriz = 2;
            cnt = 0;
            numPixelsInternos = 0;
            numPixelsBorda = 0;
            perimetro = 0;
            var convexos = PixeisConvexos(tabela);

            textos2[2].Text = convexos.Item1.ToString();
            textos2[3].Text = convexos.Item2.ToString();

            // string textin = "";
            // for (int i = 0; i < 8; i++)
            // {
            //     for (int j = 0; j < 8; j++)
            //     {
            //         textin += tabela[i, j];
            //     }
            //     GD.Print(textin);
            //     textin = "";
            // }

            (diametro[0], diametro[1]) = auxiliar.MedeDiametro(tabela);
            textos2[5].Text = diametro[0].ToString();
            textos2[6].Text = diametro[1].ToString();

            tabela = new int[8, 8];
        }
        else if (matriz == 2)
        {
            if (wait < 100)
            {
                wait += 1;
            }
            else
            {
                wait = 0;
                matriz = 0;
                linha = 0;
                coluna = 0;

                // Anotar texto1[] e texto2[] no csv
                stringCsv = numFig.ToString() + ',';
                for (int i = 0; i < 7; i++)
                {
                    stringCsv += textos1[i].Text;
                    if (i < 6)
                    {
                        stringCsv += ",";
                    }
                }
                //GD.Print(stringCsv);
                escritor.GeraCsv(stringCsv);
                stringCsv = (numFig + 1).ToString() + ',';

                for (int i = 0; i < 7; i++)
                {
                    stringCsv += textos2[i].Text;
                    if (i < 6)
                    {
                        stringCsv += ",";
                    }
                }
                //GD.Print(stringCsv);
                escritor.GeraCsv(stringCsv);
                // fim


                for (int i = 0; i < 7; i++)
                {
                    textos1[i].Text = "0";
                    textos2[i].Text = "0";
                }


                numFig += 2;
                if (numFig < 10)
                {
                    figRef = "0" + numFig.ToString() + "0" + (numFig + 1).ToString();
                }
                else
                {
                    figRef = numFig.ToString() + (numFig + 1).ToString();
                    if (figRef == "9697")
                    {
                        timer.Stop();
                        return;
                    }
                }
                sprite2D.Texture = GD.Load<Texture2D>("res://dados/Figuras" + figRef + ".png");

                quadradoDebug.Position = new Vector2(Estaticos.ORIGEMX + (Estaticos.TAMANHO / 2), Estaticos.ORIGEMY + (Estaticos.TAMANHO / 2));
            }
        }
    }

    private (int, int) PixeisConvexos(int[,] tabela)
    {
        GD.Print("Enxergado pelo Daltonista:");
        ImprimirTabela(tabela);

        ConvexHull convexHull = new();
        List<Point> hullPoints = convexHull.ComputeConvexHull(tabela);
        GD.Print(hullPoints.Count);
        int[,] envoltoriaTabela = new int[8, 8];
        //GD.Print("Points in the convex hull:");
        foreach (var point in hullPoints)
        {
            envoltoriaTabela[point.X, point.Y] = 1;
        }
        ImprimirTabela(envoltoriaTabela);

        var tabelaPreenchida = PreencherBorda(envoltoriaTabela, hullPoints, 1000);

        GD.Print("Imprimindo tabela  preenchida");

        ImprimirTabela(tabelaPreenchida);

        GD.Print("Imprimindo tabela corrigida");


        var tabelaCorrigida = CorrecaoTabela(tabelaPreenchida);


        ImprimirTabela(tabelaCorrigida);
        int pixeisInternos = 0;
        int pixeisBorda = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (tabela[i, j] == 2)
                {
                    pixeisInternos++;
                }
                else if (tabela[i, j] == 1)
                {
                    pixeisBorda++;
                }

            }
        }
        return (pixeisInternos, pixeisBorda);
    }

    private int[,] CorrecaoTabela(int[,] tabela)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (tabela[i, j] == 2)
                {
                    if (i != 0)
                    {
                        if (tabela[i - 1, j] == 0)
                        {
                            tabela[i, j] = 1;
                        }
                    }
                    else
                    {
                        tabela[i, j] = 1;
                    }

                    if (i != 7)
                    {
                        if (tabela[i + 1, j] == 0)
                        {
                            tabela[i, j] = 1;
                        }
                    }
                    else
                    {
                        tabela[i, j] = 1;
                    }

                    if (j != 0)
                    {
                        if (tabela[i, j - 1] == 0)
                        {
                            tabela[i, j] = 1;
                        }
                    }
                    else
                    {
                        tabela[i, j] = 1;
                    }

                    if (j != 7)
                    {
                        if (tabela[i, j + 1] == 0)
                        {
                            tabela[i, j] = 1;
                        }
                    }
                    else
                    {
                        tabela[i, j] = 1;
                    }

                }
            }
        }

        return tabela;
    }

    private int[,] PreencherBorda(int[,] envoltoriaTabela, List<Point> pontos, int precisao)
    {

        foreach (Point p1 in pontos)
        {
            foreach (Point p2 in pontos)
            {
                if (p1.X != p2.X && p1.Y != p2.Y)
                {
                    Vector2 p1temp = new Vector2(p1.X + 0.5f, p1.Y + 0.5f);
                    Vector2 direcao = new Vector2(((float)p2.X - p1.X) / precisao, ((float)p2.Y - p1.Y) / precisao);
                    for (int i = 0; i < precisao; i++)
                    {
                        p1temp += direcao;
                        int posX = (int)p1temp.X;
                        int posY = (int)p1temp.Y;
                        float deltaX = (float)p1temp.X - posX;
                        float deltaY = (float)p1temp.Y - posY;
                        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);


                        if (envoltoriaTabela[posX, posY] != 1 && envoltoriaTabela[posX, posY] != 2 && distance <= 0.75)
                        {
                            envoltoriaTabela[posX, posY] = 2;
                        }

                    }
                }
            }
        }
        return envoltoriaTabela;
    }


    private void ImprimirTabela(int[,] mat)
    {
        string textin = "";
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                textin += mat[i, j];
            }
            GD.Print(textin);
            textin = "";
        }
    }

    public void Daltonista(Vector2 posicao, int matriz)
    {
        int vermelho = 0;
        int branco = 0;
        int resto = 0;

        for (int i = 0; i < numPontos; i++)
        {
            cnt++;
            int meuRngX = rng.RandiRange(0, (int)Estaticos.TAMANHO - 1);
            int meuRngY = rng.RandiRange(0, (int)Estaticos.TAMANHO - 1);
            Color cor = GetViewport().GetTexture().GetImage().GetPixelv((Vector2I)GlobalToViewport(new Vector2(posicao.X + meuRngX, posicao.Y + meuRngY)));
            //GD.Print(new Vector2(posicao.X + meuRngX, posicao.Y + meuRngY));
            //GD.Print(cnt,' ', cor);
            if (cor.R == 1 && cor.G == 1 && cor.B == 1)
            {
                branco++;
            }
            else if (cor.R == 1 && cor.G == 0 && cor.B == 0)
            {
                vermelho++;
            }
            else
            {
                resto++;
            }
            //tiro.Position = new Vector2(posicao.X + meuRngX, posicao.Y + meuRngY);
        }

        tiro.Position = new Vector2(posicao.X + (Estaticos.TAMANHO) / 2, posicao.Y + (Estaticos.TAMANHO) / 2);
        //GD.Print(new Vector2(posicao.X, posicao.Y));

        if (vermelho > 0.95 * numPontos)
        {
            numPixelsInternos += 1;
            tabela[linha, coluna] = 2;
            if (matriz == 0)
            {
                textos1[0].Text = numPixelsInternos.ToString();
            }
            else if (matriz == 1)
            {
                textos2[0].Text = numPixelsInternos.ToString();
            }
        }
        else if (resto >= 0.05 * numPontos)
        {
            numPixelsBorda += 1;
            tabela[linha, coluna] = 1;
            perimetro += (float)resto * 10 / numPontos;
            perimetro = (float)Math.Round(perimetro, 2);
            if (matriz == 0)
            {
                textos1[1].Text = numPixelsBorda.ToString();
                textos1[4].Text = perimetro.ToString().Replace(',', '.');
            }
            else if (matriz == 1)
            {
                textos2[1].Text = numPixelsBorda.ToString();
                textos2[4].Text = perimetro.ToString().Replace(',', '.');
            }
        }

        //GD.Print("(", linha, ",", coluna, ")", ", V=", vermelho, ", B=", branco, ", P=", resto, ", Tot=", vermelho + branco + resto);
    }


    public Vector2 GlobalToViewport(Vector2 posicaoGlobal)
    {
        return new Vector2((posicaoGlobal.X + 1240) / 4, (posicaoGlobal.Y + 1754) / 4);
    }


    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton)
        {
            Vector2 posicaoGlobal = GetViewport().GetCamera2D().GetGlobalMousePosition();
            Vector2I posicaoRelativa = (Vector2I)eventMouseButton.Position;
            Vector2 posicaoAjustada = GlobalToViewport(posicaoGlobal);

            Vector2 viewportPosition = GetViewport().GetVisibleRect().Position;
            //GD.Print(viewportPosition);
            //GD.Print(GetViewport().GetTexture().GetImage().GetPixelv((Vector2I)posicaoAjustada));
            //GD.Print(posicaoRelativa);
            //GD.Print((Vector2I)posicaoAjustada);
        }
    }
}












// private int[,] PreencherBorda(int[,] envoltoriaTabela, List<Point> pontos, int precisao)
// {
//     foreach (Point p1 in pontos)
//     {
//         foreach (Point p2 in pontos)
//         {
//             if (p1.X != p2.X && p1.Y != p2.Y)
//             {
//                 
//             }
//         }
//     }
//     return envoltoriaTabela;
// }