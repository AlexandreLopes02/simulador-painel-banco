using System.Text.Json;
using System.Text.RegularExpressions;

namespace SimulacaoPainelDeBanco;

internal class GerenciadorBanco
{
    List<Conta> contas = new List<Conta>();
    int proximoId = 1;
    private const string CaminhoArquivo = "contas.json";
    private const decimal TAXA_SAQUE = 3m;        // taxa fixa
    private const decimal RENTABILIDADE = 0.02m;

    public GerenciadorBanco()
    {
        CarregarContasDoArquivo();
        if (contas.Count > 0)
        {
            proximoId = contas.Max(c => c.Id) + 1;
        }
    }

    //CRUD de Contas

    public void AdicionarConta()
    {
        Console.Write("Digite o CPF (com ou sem pontuação): ");
        string entradaCpf = Console.ReadLine() ?? string.Empty;
        string cpf = NormalizarCpf(entradaCpf);

        if (!CpfEhValido(cpf))
        {
            Console.WriteLine("CPF inválido.\n");
            return;
        }

        if (BuscarPorCpf(cpf) != null)
        {
            Console.WriteLine("Já existe conta com esse CPF.\n");
            return;
        }

        Console.Write("Digite o nome do titular: ");
        string nome = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome não pode ser vazio.\n");
            return;
        }

        var conta = new Conta
        {
            Id = proximoId++,
            Cpf = cpf,
            Titular = nome,
            Saldo = 0m,
            CriadaEm = DateTime.Now
        };

        contas.Add(conta);
        SalvarContasNoArquivo();
        Console.WriteLine($"Conta criada com sucesso! Id: {conta.Id}\n");
    }

    public void Depositar()
    {
        var conta = ObterContaPorCpfPerguntando("CPF para deposito: ");
        if (conta == null) return;

        
        Console.Write("Valor para deposito: R$");
        if (!decimal.TryParse(Console.ReadLine(), out decimal valor) || valor < 0) 
        {
            Console.WriteLine("Valor inválido. Informe um valor positivo.\n");
            return;
        }

        if(valor <= 0)
        {
            Console.WriteLine("Valor inválido. Informe um valor positivo.\n");
            return;
        }

        conta.Saldo += valor;
        SalvarContasNoArquivo();
        Console.WriteLine($"Depósito realizado. Saldo atual: R$ {conta.Saldo:F2}\n");
    }

    public void Sacar()
    {
        var conta = ObterContaPorCpfPerguntando("CPF para deposito: ");
        if (conta == null) return;

        if (conta.Saldo <= 0)
        {
            Console.WriteLine("Sem Saldo para saque!\n");
            return;
        }

        Console.Write("Valor do saque (taxa R$ 3 será aplicada): R$");
        if(!decimal.TryParse(Console.ReadLine(),out decimal valor) || valor < 0)
        {
            Console.WriteLine("Valor inválido. Informe um valor positivo.\n");
            return;
        }


        if(valor <= 0)
        {
            Console.WriteLine("Valor inválido. Informe um valor positivo.\n");
            return;
        }

        decimal total = valor + TAXA_SAQUE;
        if (conta.Saldo < total)
        {
            Console.WriteLine($"Saldo insuficiente para saque. Seu saldo é de R${conta.Saldo:F2}\n");
            return;
        }

        conta.Saldo -= total;
        SalvarContasNoArquivo();
        Console.WriteLine($"Saque de R$ {valor:F2} realizado (taxa R$ {TAXA_SAQUE:F2}). Saldo: R$ {conta.Saldo:F2}\n");
    }

    public void VerSaldo()
    {
        var conta = ObterContaPorCpfPerguntando("CPF para consulta: ");
        if (conta == null) return;

        Console.WriteLine($"Titular: {conta.Titular} | Saldo: R$ {conta.Saldo:F2}\n");
    }

    public void Investir()
    {
        var conta = ObterContaPorCpfPerguntando("CPF para deposito: ");
        if (conta == null) return;

        if (conta.Saldo <= 0)
        {
            Console.WriteLine("Sem Saldo para investir!\n");
            return;
        }

        Console.Write("Valor para investir: R$");
        if (!decimal.TryParse(Console.ReadLine(), out decimal valor) || valor < 0)
        {
            Console.WriteLine("Valor inválido. Informe um valor positivo.\n");
            return;
        }

        if(valor <= 0)
        {
            Console.WriteLine("Valor invalido digite um numero valor(positivo)!");
            return;
        }

        if (valor > conta.Saldo)
        {
            Console.WriteLine($"Saldo insuficiente. Saldo atual: R$ {conta.Saldo:F2}\n");
            return;
        }

        decimal retorno = Math.Round(valor * RENTABILIDADE, 2);
        conta.Saldo = conta.Saldo - valor + (valor + retorno);
        SalvarContasNoArquivo();
        Console.WriteLine($"Investimento aplicado! Retorno: R$ {retorno:F2}. Saldo: R$ {conta.Saldo:F2}\n");

    }

    public void ListarContas()
    {
        if (contas.Count == 0)
        {
            Console.WriteLine("Não há contas cadastradas.\n");
            return;
        }

        Console.WriteLine("=== CONTAS CADASTRADAS ===");
        foreach (var c in contas.OrderBy(c => c.Id))
            c.Exibir();
        Console.WriteLine();
    }

    // --------------------- Auxiliares ---------------------

    private Conta? ObterContaPorCpfPerguntando(string prompt)
    {
        Console.Write(prompt);
        string entradaCpf = Console.ReadLine() ?? string.Empty;
        string cpf = NormalizarCpf(entradaCpf);

        if (!CpfEhValido(cpf))
        {
            Console.WriteLine("CPF inválido.\n");
            return null;
        }

        var conta = BuscarPorCpf(cpf);
        if (conta == null)
        {
            Console.WriteLine("Conta não encontrada.\n");
            return null;
        }

        return conta;
    }

    private Conta? BuscarPorCpf(string cpfSomenteDigitos)
    {
        return contas.FirstOrDefault(c => c.Cpf == cpfSomenteDigitos);
    }

    // Remove tudo que não for dígito
    private string NormalizarCpf(string entrada)
    {
        return Regex.Replace(entrada ?? string.Empty, @"\D", "");
    }

    // Validação brasileira de CPF com DV
    private bool CpfEhValido(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        // mantém apenas dígitos
        cpf = NormalizarCpf(cpf);
        if (!Regex.IsMatch(cpf, @"^\d{11}$")) return false;

        // rejeita todos os dígitos iguais (ex: 00000000000)
        if (new string(cpf[0], cpf.Length) == cpf) return false;

        // calcula DV1
        int soma = 0;
        for (int i = 0, peso = 10; i < 9; i++, peso--)
            soma += (cpf[i] - '0') * peso;

        int dv1 = (soma * 10) % 11;
        if (dv1 == 10) dv1 = 0;
        if (dv1 != (cpf[9] - '0')) return false;

        // calcula DV2
        soma = 0;
        for (int i = 0, peso = 11; i < 10; i++, peso--)
            soma += (cpf[i] - '0') * peso;

        int dv2 = (soma * 10) % 11;
        if (dv2 == 10) dv2 = 0;
        if (dv2 != (cpf[10] - '0')) return false;

        return true;
    }


    //Persistencia de dados em arquivo JSON

    private void SalvarContasNoArquivo()
    {
        string json = System.Text.Json.JsonSerializer.Serialize(contas);
        File.WriteAllText(CaminhoArquivo, json);
    }

    private void CarregarContasDoArquivo()
    {
        if (!File.Exists(CaminhoArquivo))
        {
            contas = new List<Conta>();
            return;
        }

        try
        {
            string json = File.ReadAllText(CaminhoArquivo);
            var dados = JsonSerializer.Deserialize<List<Conta>>(json);
            contas = dados ?? new List<Conta>();
        }
        catch
        {
            contas = new List<Conta>();
        }
    }
}
