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
        public string GetIterationData()
        {
            return "alpha = " + Estaca_alpha + " beta = " + Estaca_beta + " X = " + No_estaca_x + " Y = " + No_estaca_y;
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

