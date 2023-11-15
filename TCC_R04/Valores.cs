using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCC_R04
{
    public class Variaveis
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double Estaca_alpha { get; set; }
        public double Estaca_beta { get; set; }
        public double H_solo { get; set; }
        public double H_laje_solo { get; set; }
        public double H_agua { get; set; }
        public double H_conc { get; set; }
        public double No_estaca_x { get; set; }
        public double No_estaca_y { get; set; }
        public double Rigidez_i { get; set; }
        public double Rigidez_f { get; set; }
        public double No_cabeco_x { get; set; }
        public double No_cabeco_y { get; set; }
        public double Sobrecarga { get; set; }
        public double Corrente_long { get; set; }
        public double Corrente_transv { get; set; }
        public double Forca_cab { get; set; }
        public Variaveis(double a, double b, double c, double estaca_alpha, double estaca_beta, double h_solo, double h_laje_solo, double h_agua, double h_conc, double no_estaca_x, double no_estaca_y, double rigidez_i, double rigidez_f, double no_cabeco_x, double no_cabeco_y, double sobrecarga, double corrente_long, double corrente_transv, double forca_cab)
        {
            A = a;
            B = b;
            C = c;
            Estaca_alpha = estaca_alpha;
            Estaca_beta = estaca_beta;
            H_solo = h_solo;
            H_laje_solo = h_laje_solo;
            H_agua = h_agua;
            H_conc = h_conc;
            No_estaca_x = no_estaca_x;
            No_estaca_y = no_estaca_y;
            Rigidez_i = rigidez_i;
            Rigidez_f = rigidez_f;
            No_cabeco_x = no_cabeco_x;
            No_cabeco_y = no_cabeco_y;
            Sobrecarga = sobrecarga;
            Corrente_long = corrente_long;
            Corrente_transv = corrente_transv;
            Forca_cab = forca_cab;
        }
        public double H_total()
        {
            return H_solo + H_laje_solo;
        }
        public double Estaca_comp()
        {
            return H_total() / Math.Sin(Math.PI * Estaca_alpha / 180);
        }
        public double Estaca_comp_conc()
        {
            return H_conc / Math.Sin(Math.PI * Estaca_alpha / 180);
        }
        public double H_cabeco()
        {
            return C / 2 + 0.25;
        }
    }
    public class Variaveis_iter
    {
        public double Estaca_alpha_passo { get; set; }
        public double Estaca_beta_passo { get; set; }
        public double No_estaca_x_passo { get; set; }
        public double No_estaca_y_passo { get; set; }
        public double Estaca_alpha_iter { get; set; }
        public double Estaca_beta_iter { get; set; }
        public double No_estaca_x_iter { get; set; }
        public double No_estaca_y_iter { get; set; }
        public Variaveis_iter(double estaca_alpha_passo, double estaca_beta_passo, double no_estaca_x_passo, double no_estaca_y_passo, double estaca_alpha_iter, double estaca_beta_iter, double no_estaca_x_iter, double no_estaca_y_iter)
        {
            Estaca_alpha_passo = estaca_alpha_passo;
            Estaca_beta_passo = estaca_beta_passo;
            No_estaca_x_passo = no_estaca_x_passo;
            No_estaca_y_passo = no_estaca_y_passo;
            Estaca_alpha_iter = estaca_alpha_iter;
            Estaca_beta_iter = estaca_beta_iter;
            No_estaca_x_iter = no_estaca_x_iter;
            No_estaca_y_iter = no_estaca_y_iter;
        }
    }

    public class Resultados
    {
        public string Elemento { get; set; }
        public bool Estaca_Laje { get; set; } // True = Estaca ; False = Laje
        public string Envoltoria { get; set; }
        public bool ForcaInterna_Deformacao { get; set; } //True = Força interna ; False = Deformação
        public string Magnitude { get; set; }
        public double Minimo { get; set; }
        public double Maximo { get; set; }
        public Resultados(string elemento, bool estaca_laje, string envoltoria, bool forcainterna_deformacao, string magnitude, double minimo, double maximo)
        {
            Elemento = elemento;
            Estaca_Laje = estaca_laje;
            Envoltoria = envoltoria;
            ForcaInterna_Deformacao = forcainterna_deformacao;
            Magnitude = magnitude;
            Minimo = minimo;
            Maximo = maximo;
        }
        public double Maximo_abs()
        {
            return Math.Max(Math.Abs(Minimo), Math.Abs(Maximo));
        }
        public string Tipo_Envoltoria() //ELU = Estado Limite Último, ELSR = Estado Limite de Serviço Raras,
        {                               // ELSQ = Estado Limite de Serviço Quase Permanente, ELSF = Estado Limite de Serviço Frequente
            string nome1 = Envoltoria.Substring(0, 3);
            string nome2 = Envoltoria.Substring(0, 4);
            string nome3;
            if (nome1 == "ELU") { nome3 = nome1; } else { nome3 = nome2; }
            return nome3;

        }
        public string Categoria()
        {
            string cat;
            if (Tipo_Envoltoria() == "ELU") { cat = "ELU  "; } else { cat = Tipo_Envoltoria() + " "; }
            if (Estaca_Laje == true) { cat += "Estaca "; } else { cat += "Laje   "; }
            if (ForcaInterna_Deformacao == true) { cat += "Força Interna "; } else { cat += "Deformação    "; }
            cat += Magnitude.PadLeft(5);

            return cat;
        }

    }
    public class Resultados_el_env_FiD_mag
    {
        public string Categoria { get; set; }
        public double Minimo { get; set; }
        public double Maximo { get; set; }
        public Resultados_el_env_FiD_mag(string categoria, double minimo, double maximo)
        {
            Categoria = categoria;
            Minimo = minimo;
            Maximo = maximo;
        }
    }

    //public class Resultados_el_env_FiD_mag
    //{
    //    public bool Estaca_Laje { get; set; } // True = Estaca ; False = Laje
    //    public string Tipo_envoltoria { get; set; }
    //    public bool ForcaInterna_Deformacao { get; set; } //True = Força interna ; False = Deformação
    //    public string Magnitude { get; set; }
    //    public double Minimo { get; set; }
    //    public double Maximo { get; set; }
    //    public Resultados_el_env_FiD_mag( bool estaca_laje, string tipo_envoltoria, bool forcainterna_deformacao, string magnitude, double minimo, double maximo)
    //    {
    //        Estaca_Laje = estaca_laje;
    //        Tipo_envoltoria = tipo_envoltoria;
    //        ForcaInterna_Deformacao = forcainterna_deformacao;
    //        Magnitude = magnitude;
    //        Minimo = minimo;
    //        Maximo = maximo;
    //    }
    //    public double Maximo_abs()
    //    {
    //        return Math.Max(Math.Abs(Minimo), Math.Abs(Maximo));
    //    }

    //}
    internal class Valores
    {

        /*
        Maior dimensão(eixo X) = a
        Menor dimensão(eixo Y) = b
        Espessura(eixo Z) = c
        */

        public static double a = 6; public static double b = 5; public static double c = 1.5;

        /*
        Nós das estacas: vamos definir o nó mais próximo da origem, as demais serão definidas por simetria.
        no_estaca_x é a coordenada x e no_estaca_y é a coordenada y do nó mais próximo.
            */

        public static double no_estaca_x = 1.5; public static double no_estaca_y = 1;

        /*
        estaca_comp: comprimento total da estaca a partir do plano XY;
        estaca_alpha: ângulo entre a estaca e o plano XY (0° a estaca pertenceria ao plano XY, 90° a estaca estaria na vertical);
        estaca_beta: ângulo entre a projeção da estaca no plano XY e o eixo Y (0° a projeção estaria sobre o eixo Y, 90° a projeção estaria sobre o eixo X);
        estaca_comp_conc: comprimento da estaca concretada.
        */
        public static double h_solo = 7.7; public static double h_laje_solo = 29; public static double h_agua = 20; public static double h_conc = 9.7;
        public static double h_total = h_solo + h_laje_solo;
        public static double estaca_alpha = 75; public static double estaca_beta = 45;
        public static double estaca_comp = h_total / Math.Sin(Math.PI * estaca_beta / 180);
        public static double estaca_comp_conc = h_conc / Math.Sin(Math.PI * estaca_beta / 180);

        /*
        Nós dos cabeços de amarração: vamos definir o nó mais próximo da origem, as demais serão definidas por simetria.
        no_estaca_x é a coordenada x e no_estaca_y é a coordenada y do nó mais próximo.
            */

        public static double no_cabeco_x = 1.5; public static double no_cabeco_y = b / 2; public static double cabeco_h = c / 2 + 0.25;

        /*
        Profundidade do solo (profundidade em que a extremidade da estaca se apoia, em relação ao leito)
        Rigidez inicial
        Rigidez final
            */

        public static double rigidez_i = 4.57e6; public static double rigidez_f = 75.405e6;

        //Profundidade da água
        //Sobrecarga em N/m²
        //Força da corrente transversal e longitudinal em N/m²

        public static double sobrecarga = -5e3; public static double corrente_long = 320; public static double corrente_transv = 320; public static double forca_cab = 10e3;


    }
}
