workspace "Fluxo de Caixa" "Sistema de Fluxo de Caixa" {

    !identifiers hierarchical

    model {
		user = person "Usuário"
        
		fluxoCaixa = softwareSystem "Fluxo de Caixa" {
            webApp = container "Web App" "Responsável por entregar o conteúdo estático para o navegador e direcionar chamadas para o os serviços" "Blazor" "WebApp" 
            
			db = container "Banco de Dados" "Armazena registros de crédito e débito" "PostgreSQL" "Database"
			
			msControleLancamentos = container "Serviço Controle Lancamentos" "Processa solicitações de crédito e débito" "C# .NET" "FluxoCaixaService"
            
            msSaldoConsolidado = container "Serviço Consolidado Diário" "Processa solicitações de crédito e débito e faz a consolidação do saldo diário" "C# .NET" "SaldoConsolidadoService"

			broker = container "Broker" "Recebe eventos de débito e crédito e envia para serviços inscritos" "RabbitMQ" "Broker"

            cache = container "Cache" "Armazena saldo consolidado dos últimos 30 dias" "Redis" "Cache"
        }
		
        user -> fluxoCaixa.webApp "Solicita operações de crédito e débito e visualiza relatórios" "HTTPS"
		
        fluxoCaixa.webApp -> fluxoCaixa.msControleLancamentos "Solicita operações de crédito e débito" "gRPC"

        fluxoCaixa.webApp -> fluxoCaixa.msSaldoConsolidado "Solicita relatório de saldo consolidado por período" "gRPC"

		fluxoCaixa.msSaldoConsolidado -> fluxoCaixa.db "Persiste registros de crédito e débito"

        fluxoCaixa.db -> fluxoCaixa.msSaldoConsolidado "Lê registros de crédito e débito"

        fluxoCaixa.msControleLancamentos -> fluxoCaixa.broker "Envia eventos de débito e crédito"

        fluxoCaixa.broker -> fluxoCaixa.msSaldoConsolidado "Recebe eventos de débito e crédito"

        fluxoCaixa.msSaldoConsolidado -> fluxoCaixa.cache "Persiste registros de crédito e débito"

        fluxoCaixa.cache -> fluxoCaixa.msSaldoConsolidado "Lê registros de crédito e débito"
    }

    views {
        systemContext fluxoCaixa "Contexto" {
            include *
            autolayout lr
        }

        container fluxoCaixa "Containers" {
            include *
            autolayout lr
        }

        styles {
            element "Element" {
                color #ffffff
            }
            element "Person" {
                background #4876FF
                shape person
            }
            element "Software System" {
                background #4876FF
            }
            element "Cache" {
                shape cylinder
                background #EE7AE9
            }
            element "Database" {
                shape cylinder
                background #EE7AE9
            }
            element "Broker" {
                shape Pipe
                background #C1CDCD
            }
            element "FluxoCaixaService" {
                shape Hexagon
                background #43CD80
            }
             element "SaldoConsolidadoService" {
                shape Hexagon
                background #EE7AE9
            }
            element "WebApp" {
                shape WebBrowser
                background #00BFFF

            }
        }
    }

    configuration {
        scope softwaresystem
    }

}