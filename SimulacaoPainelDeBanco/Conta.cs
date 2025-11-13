namespace SimulacaoPainelDeBanco;

internal class Conta
{
    public int Id { get; set; }
    public string Cpf { get; set; } = string.Empty;     
    public string Titular { get; set; } = string.Empty;
    public decimal Saldo { get; set; } = 0m;
    public DateTime CriadaEm { get; set; } = DateTime.Now;

    public void Exibir()
    {
        Console.WriteLine($"Id: {Id} | CPF: {Cpf} | Titular: {Titular} | Saldo: R$ {Saldo:F2}");
    }
}
