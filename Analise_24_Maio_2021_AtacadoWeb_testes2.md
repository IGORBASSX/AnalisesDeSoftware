## Produtos com Promoção + Pack/ Desconto Atacado

![image](https://user-images.githubusercontent.com/50722662/126704493-38e2b307-d932-47aa-84e9-aab9192d8568.png)



1. Mensagens de aviso ao cadastrar um produto com 2 tipos de promoção
    * Mensagem ao cadastrar promoção (teste ok)
   
   ![image](https://user-images.githubusercontent.com/80394522/126570266-81f09b1a-a046-44db-b0c7-dc1b7c72700a.png)
   
    * Mensagem no prorrogar promoção (teste ok)

   ![image](https://user-images.githubusercontent.com/80394522/126570848-d1619472-9d21-4b21-83c4-7e16b05ab02f.png)

    * Mensagem ao cadastrar pack virtual (teste ok)

   ![image](https://user-images.githubusercontent.com/80394522/126572067-ef8e1ca9-9b7a-4e5d-ab5b-9b6e3ee7a6ec.png)

    * Mensagem no prorrogar pack virtual (teste ok)

   ![image](https://user-images.githubusercontent.com/80394522/126576420-c2b48e25-6e53-4ba7-9886-805f4c3e6f6b.png)   

    * Mensagem ao cadastrar desconto atacado (teste ok)

   ![image](https://user-images.githubusercontent.com/80394522/126576580-016d64c9-0bec-449d-9634-5b4915fe1294.png)

    * Mensagem ao prorrogar encarte em que uma promoção sobrescreva um pack/desconto atacado (teste ok)
    
   ![image](https://user-images.githubusercontent.com/80394522/126577627-99768c43-3ca5-4cc8-b2cc-ebb3733bdb03.png)


    * Mensagem ao prorrogar encarte em que um pack sobrescreva uma promoção (teste ok)

    ![image](https://user-images.githubusercontent.com/80394522/126576977-c9752870-0b9d-4821-9a12-c2bce5b646d6.png)


1. PDV - produto com pack ou desconto atacado cadastrado para uma determinada loja deverá ser vendido com preço de promoção

## Maximizar

1. Maximizar controle de entradas deverá funcionar em diversas resoluções com ou sem a configuração usa atacado  (teste ok)

1600X900 SEM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126577922-379fe4b3-e374-42e9-9fc6-46a1e2882885.png)

1600X900 COM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126577960-bbf7cfd9-5c3a-4029-b77b-cc7718696c64.png)

1366x768 SEM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126578087-478fc2c2-5e87-4f8a-aff2-193f19dee031.png)

1366x768 COM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126578069-64f55c14-8e8a-4399-bb71-83c2dc0af357.png)

1024x768 SEM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126578170-aa3e3d8a-93a4-4d86-b506-c8213a69e138.png)

1024x768 COM A CONFIGUIRAÇÃO "USA DESCONTO ATACADO"
![image](https://user-images.githubusercontent.com/80394522/126578192-ba78123f-4b59-499b-8cc5-3733f6784d1e.png)


## Grupo de produtos

1. Campo para código do grupo no ecommerce

## Controle de entradas

1. Se não marcar a configuração "Exibir preços de Atacado" a tela deverá funcionar como antes, com melhorias no maximizar e nas colorações e tooltip

1. Ao ativar a configuração do controle de entradas usa preço atacado 
    * permitir cadatrar as lojas e arredondamento para os novos descontos para atacado que serão cadastrados na tela
    * Ao salvar as configurações de arredondamento e loja deverão ser refletidas em todos os descontos para atacado cadastrados
    * mostrar coluna de desconto atacado na grade
    * menu para add ou remover desconto atacado nos produtos 
    * ao alterar valor, margem ou markup do produto deverá refletir no preço do atacado
    * ao alterar valor, margem ou markup do desconto atacado deverá abrir a tela de alteração já com o valor digitado
    * duplo clique no valor, margem ou markup do desconto atacado deverá abrir a tela de alteração
    * Atalhos (A) e (DEL) para inserir ou remover atacado
    * combobbox deve possibilitar alteração do desconto de atacado
    * ao alterar o combobox deverá ser recalculado o markup, margem e valor de venda 
    * novo do combobox abre a tela para cadastrar novo desconto atacado

1. Detalhes da tela
    * Na grade o cálculo da porcentagem do desconto atacado deverá levar em consideração o arredondamento configurado
    * Ao aplicar valores deverá ser salvo o desconto atacado de acordo todas as lojas configuradas
    * Ao aplicar valores deverá ser desativado os descontos para atacado que não são mais utilizandos, visando ter o menor número possível
    * Ao listar na grade o desconto atacado deverá ser listado de acordo com as lojas configuradas
    * Se o produto estar repetidos na grade para lojas diferentes que estejam configuradas para utilizar o desconto atacado, deverá ser 
    inserido, removido e alterado em todas as ocorrências
    * Ao filtrar um número grande de registros deverá ser exibida uma mensagem e não dar erro na tela
    * Foram feitos diversos tratamentos para a coloração de seleção do grid (azul), não deverá se perder ou se repetir

1. Tooltip + coloração do grid
    * produto em promoção

   ![image](https://user-images.githubusercontent.com/80394522/126570751-4b63127d-4b66-46df-9f84-d67a6751457c.png)

    * produto com pack
    * diferenças entre markup/ markup ideal e margem/ margem ideal de acordo com a diferença configurada 
    * produtos fora do mix 
    * custo gerencial de notas que não alteram custo
    * preço de venda alterado - neste caso agora deverá ficar alterado até o envio de carga para os pdvs

1. Impressão
    * o relatório deverá imprimir também os registros relacionados a desconto atacado bem como markup, marge e valor atacado
    * Existem 2 relatórios com ou sem o agrupamento por fornecedor
    
## Controle de entradas - Tela de cadastro/alteração do desconto para atacado

1. Detalhes da tela
    * Na tela de alteração ao digitar um valor de atacado deverá sugerir uma porcentagem calculada de acordo com o arredondamento configurado. 
    Buscando sempre uma porcentagem que não fique quebrada
    * Na tela de alteração ao digitar uma porcentagem o valor de atacado deverá ser calculado de acordo com o arredondamento configurado
    * Ao editar a porcentagem o valor de atacado deverá ser calculado de acordo com o arredondamento configurado    
    * Ao cadastrar um novo desconto atacado pelo atalho (A), pelo menu inserir, ou pelo novo do combo, a quantidade do produto deverá ser preencida de acordo com a embalagem do fornecedor cadastrada


## Banco de Dados 

1. Inserção de desconto atacado em um produto
   * Deve criar um registro com código 6 na tabela DriveFilaExportação
   * Deve exibir o registro com o valor correto na view VW_ProdutosAtacado

1. Alteração de desconto atacado em um produto
   * Deve excluir e criar um registro com código 6 na tabela DriveFilaExportação
   * Deve exibir o registro com o valor correto na view VW_ProdutosAtacado

1. Produto com promoção
   * Deve exibir o registro com o valor da promoção na view VW_ProdutosAtacado
