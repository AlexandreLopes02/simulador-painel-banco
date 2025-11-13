using SimulacaoPainelDeBanco;

internal class Program
{
    private static void Main(string[] args)
    {
        GerenciadorBanco gerenciador = new GerenciadorBanco();
        int opcao = 0;

        while(opcao != 8)
        {
            Console.WriteLine("=== PAINEL DO BANCO ===");
            Console.WriteLine("1 - Criar conta");
            Console.WriteLine("2 - Depositar");
            Console.WriteLine("3 - Sacar (taxa R$ 3)");
            Console.WriteLine("4 - Ver saldo");
            Console.WriteLine("5 - Investir (+2% imediato)");
            Console.WriteLine("6 - Excluir conta");
            Console.WriteLine("7 - Listar contas");
            Console.WriteLine("8 - Sair");
            Console.Write("Escolha uma opção: ");

            if (!int.TryParse(Console.ReadLine(), out opcao))
            {
                Console.WriteLine("Opção inválida.\n");
                continue;
            }

            Console.Clear();

            switch (opcao)
            {
                case 1:
                    gerenciador.AdicionarConta();
                    break;
                case 2:
                    gerenciador.Depositar();
                    break;
                case 3:
                    gerenciador.Sacar();
                    break;
                case 4:
                    gerenciador.VerSaldo();
                    break;
                case 5:
                    
                    break;
                case 6:
                    
                    break;
                case 7:
                    gerenciador.ListarContas();
                    break;
                case 0:
                    Console.WriteLine("Saindo do sistema...");
                    break;
                default:
                    Console.WriteLine("Opção inválida.\n");
                    break;
            }
        }
    }
}
