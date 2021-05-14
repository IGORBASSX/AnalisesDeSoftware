using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telecon.Genericos.Controles.Classes;
using tele = Telecon.Genericos.Classes;
using System.Drawing.Printing;

namespace Telecon.Genericos.Controles
{
    [DefaultEvent("SelecionouLinha")]
    public partial class DataGridViewPersonalizado : DataGridView
    {
        #region Vari�veis e Propriedades

        private PictureBox _logotipo;
        Classes.DataGridViewPrinter _dataGridViewPrinter;
        private System.Drawing.Printing.PrintDocument _documentoImpressao;
        Dictionary<int, Classes.ItemComboBusca.TipoDados> _tipoColunas = new Dictionary<int, Classes.ItemComboBusca.TipoDados>();
        private string _ultimoValorCelula;
        private int _decimais = 2;

        public void DefinirTipoColuna(int v, object itemcombobusca)
        {
            throw new NotImplementedException();
        }

        private bool _marcarCelulasCliqueCabecalho = true;

        private const int GRID_CHECKED = -1;
        private const int GRID_UNCHECKED = 0;

        public delegate void SelecionouLinhaHandler(DataGridViewPersonalizado dgv, DataGridViewRow linha);

        public event SelecionouLinhaHandler SelecionouLinha;

        /// <summary>
        /// Define o tipo de dados da coluna.
        /// </summary>
        /// <param name="coluna">�ndice da coluna.</param>
        /// <param name="tipo">Tipo de dados.</param>
        public void DefinirTipoColuna(int coluna, Classes.ItemComboBusca.TipoDados tipo)
        {
            _tipoColunas[coluna] = tipo;
        }

        /// <summary>
        /// Deixa uma c�lula em negrito
        /// </summary>
        /// <param name="linha">Linha da c�lula que ser� formatada</param>
        /// <param name="coluna">Coluna da c�lula que ser� formatada</param>
        public void DefinirCelulaNegrito(int linha, int coluna)
        {
            base.Rows[linha].Cells[coluna].Style.Font = ObterFonteComNegrito;
        }

        public void DefinirCelulaCorFonte(int linha, int coluna, Color cor)
        {
            base.Rows[linha].Cells[coluna].Style.ForeColor = cor;
        }

        public void DefinirCelulaRegular(int linha, int coluna)
        {
            base.Rows[linha].Cells[coluna].Style.Font = ObterFonteSemNegrito;
        }

        public void DefinirCelulaRiscada(int linha, int coluna)
        {
            base.Rows[linha].Cells[coluna].Style.Font = ObterFonteComRiscado;
        }

        public void DefinirLinhaRiscada(int linha)
        {
            foreach (DataGridViewCell celula in this.Rows[linha].Cells)
            {
                celula.Style.Font = ObterFonteComRiscado;
            }

        }

        /// <summary>
        /// Retorna um objeto formatado com a fonte padr�o do grid e a op��o de negrito ativada
        /// </summary>
        public Font ObterFonteComNegrito
        {
            get
            {
                return new Font(base.DefaultCellStyle.Font.Name, base.DefaultCellStyle.Font.Size, (FontStyle)(Convert.ToInt32(FontStyle.Bold)), GraphicsUnit.Point);
            }
        }

        public Font ObterFonteComRiscado
        {
            get
            {
                return new Font(base.DefaultCellStyle.Font.Name, base.DefaultCellStyle.Font.Size, (FontStyle)(Convert.ToInt32(FontStyle.Strikeout)), GraphicsUnit.Point);
            }
        }

        public Font ObterFonteSemNegrito
        {
            get
            {
                return new Font(base.DefaultCellStyle.Font.Name, base.DefaultCellStyle.Font.Size, (FontStyle)(Convert.ToInt32(FontStyle.Regular)), GraphicsUnit.Point);
            }
        }

        /// <summary>
        /// Retorna o tipo de dados de uma coluna.
        /// </summary>
        /// <param name="coluna">�ndice da coluna.</param>
        /// <returns>Retorna o tipo de dados de uma coluna.</returns>
        public Classes.ItemComboBusca.TipoDados ObterTipoColuna(int coluna)
        {
            return _tipoColunas[coluna];
        }

        /// <summary>
        /// Retorna o valor da c�lula antes da edi��o.
        /// </summary>
        public string UltimoValorCelula
        {
            get { return _ultimoValorCelula; }
        }

        public DataGridViewRow UltimaLinha
        {
            get { return RowCount > 0 ? Rows[RowCount - 1] : null; }
        }

        /// <summary>
        /// Retorna se existe uma linha selecionada
        /// </summary>
        public bool LinhaEstaSelecionada
        {
            get
            {
                if (this.SelectionMode == DataGridViewSelectionMode.FullRowSelect)
                    return SelectedRows.Count > 0;
                else
                    return SelectedCells.Count > 0;
            }
        }

        /// <summary>
        /// Retorna a primeira linha selecionada
        /// </summary>
        public DataGridViewRow LinhaSelecionada
        {
            get
            {
                if (this.SelectionMode == DataGridViewSelectionMode.CellSelect)
                    return LinhaEstaSelecionada ? Rows[SelectedCells[0].RowIndex] : null;
                else
                    return LinhaEstaSelecionada ? Rows[SelectedCells[0].RowIndex] : null;
            }
        }



        /// <summary>
        /// N�mero de casas decimais em caso da coluna for do tipo moeda.
        /// </summary>
        public int Decimais
        {
            get { return _decimais; }
            set { _decimais = value; }
        }

        #endregion

        #region Contrutores

        /// <summary>
        /// Este construtor apenas chama o procedimento InitializeComponent do controle original
        /// </summary>
        public DataGridViewPersonalizado()
        {
            InitializeComponent();

            this.RowHeadersVisible = false;
            this.AllowUserToResizeRows = false;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.CellDoubleClick += delegate (object sender, DataGridViewCellEventArgs args)
                                        {
                                            ChamarSelecionouLinha();
                                        };
            this.KeyDown += delegate (object sender, KeyEventArgs args)
                                 {
                                     if (args.KeyCode == Keys.Enter)
                                         ChamarSelecionouLinha();
                                 };

            DataError += new DataGridViewDataErrorEventHandler(DataGridViewPersonalizado_DataError);
        }

        void DataGridViewPersonalizado_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string aux = e.Exception.Message;//evita a mensagem de erro ao editar, caso exista uma coluna checkbox            
        }

        private void ChamarSelecionouLinha()
        {
            if (SelecionouLinha != null)
            {
                if (!LinhaEstaSelecionada)
                    return;
                SelecionouLinha(this, LinhaSelecionada);
            }
        }

        /// <summary>
        /// Logotipo usado na impress�o do grid.
        /// </summary>
        public PictureBox Logotipo
        {
            get { return _logotipo; }
            set { _logotipo = value; }
        }
        /// <summary>
        /// Determina se grid deve marcar/desmarcar todas as c�lulas da coluna ao clicar no cabe�alho. Padr�o true
        /// </summary>
        public bool MarcarCelulasCliqueCabecalho
        {
            get { return _marcarCelulasCliqueCabecalho; }
            set { _marcarCelulasCliqueCabecalho = value; }
        }

        /// <summary>
        /// Text matrix da �ltima linha
        /// </summary>        
        /// <param name="coluna">Coluna do texto</param>
        /// <returns>O texto contido nesta coluna na �ltima linha</returns>
        public string this[int coluna]
        {
            get { return ObterTextMatrix(Rows.Count - 1, coluna); }
            set { DefinirTextMatrix(Rows.Count - 1, coluna, value); }
        }

        /// <summary>
        /// Text matrix da linha (esta propriedade j� existe no grid orignal, por�m n�o retorna os dados corretos
        /// </summary>        
        /// <param name="linha">Linha do texto</param>
        /// <param name="coluna">Coluna do texto</param>
        /// <returns>O texto contido nesta coluna na �ltima linha</returns>
        new public string this[int linha, int coluna]
        {
            get { return ObterTextMatrix(linha, coluna); }
            set { DefinirTextMatrix(linha, coluna, value); }
        }

        ///// <summary>
        ///// Equivalente ao Text Matrix
        ///// </summary>
        ///// <param name="linha">Linha onde est� localizado o valor</param>
        ///// <param name="coluna">Coluna onde est� localizado o valor</param>
        ///// <returns>O texto contido nesta c�lula</returns>
        //public string this[int linha, int coluna]
        //{            
        //    get { return ObterTextMatrix(linha, coluna); }
        //    set { DefinirTextMatrix(linha, coluna, value); }
        //}

        #endregion

        #region M�todos

        /// <summary>
        /// Inicializa as vari�veis para impress�o do grid, sem o nome do estabelecimento/empresa.
        /// </summary>
        /// <param name="titulo">T�tulo que ser� impresso</param>
        /// <param name="paisagem">True para paisagem, ou False para retrato.</param>
        /// <returns>True caso sucesso, ou False caso falhe.</returns>
        private bool ConfigurarImpressao(string titulo, bool paisagem)
        {
            return ConfigurarImpressao(titulo, paisagem, "");
        }


        private bool ConfigurarImpressao(string titulo, bool paisagem, string empresa)
        {
            return ConfigurarImpressao(titulo, paisagem, empresa, "");
        }

        /// <summary>
        /// Inicializa as vari�veis para impress�o do grid.
        /// </summary>
        /// <param name="titulo">T�tulo que ser� impresso.</param>
        /// <param name="paisagem">True para paisagem, ou False para retrato.</param>
        /// <param name="empresa">Nome do estabelecimento do cliente.</param>
        /// <returns>True caso sucesso, ou False caso falhe.</returns>
        private bool ConfigurarImpressao(string titulo, bool paisagem, string empresa, string detalhamento)
        {
            return ConfigurarImpressao(titulo, paisagem, empresa, detalhamento, null);
        }

        /// <summary>
        /// Inicializa as vari�veis para impress�o do grid.
        /// </summary>
        /// <param name="titulo">T�tulo que ser� impresso.</param>
        /// <param name="paisagem">True para paisagem, ou False para retrato.</param>
        /// <param name="empresa">Nome do estabelecimento do cliente.</param>
        /// <param name="configuracaoImpressao">Configura��o para impress�o. Passar null caso deseja perguntar a configura��o ao usu�rio.</param>
        /// <returns>True caso sucesso, ou False caso falhe.</returns>
        private bool ConfigurarImpressao(string titulo, bool paisagem, string empresa, string detalhamento, PrinterSettings configuracaoImpressao)
        {
            if (this._documentoImpressao == null)
            {
                this._documentoImpressao = new System.Drawing.Printing.PrintDocument();
                this._documentoImpressao.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this._documentoImpressao_PrintPage);
            }
            if (configuracaoImpressao == null)
            {
                PrintDialog _caixaDialogoImpressao = new PrintDialog();
                _caixaDialogoImpressao.AllowCurrentPage = false;
                _caixaDialogoImpressao.AllowPrintToFile = false;
                _caixaDialogoImpressao.AllowSelection = false;
                _caixaDialogoImpressao.AllowSomePages = false;
                _caixaDialogoImpressao.PrintToFile = false;
                _caixaDialogoImpressao.ShowHelp = false;
                _caixaDialogoImpressao.ShowNetwork = false;

                if (_caixaDialogoImpressao.ShowDialog() != DialogResult.OK)
                    return false;
                configuracaoImpressao = _caixaDialogoImpressao.PrinterSettings;
            }
            _documentoImpressao.DocumentName = titulo;
            _documentoImpressao.PrinterSettings = configuracaoImpressao;
            _documentoImpressao.DefaultPageSettings = configuracaoImpressao.DefaultPageSettings;
            _documentoImpressao.DefaultPageSettings.Margins = new Margins(40, 40, 40, 40);
            _documentoImpressao.PrinterSettings.DefaultPageSettings.Landscape = paisagem;

            Font oFonte = new Font("Arial", 20, (FontStyle)(Convert.ToInt32(FontStyle.Bold) + Convert.ToInt32(FontStyle.Italic)), GraphicsUnit.Point);

            _dataGridViewPrinter = new Classes.DataGridViewPrinter(this, _documentoImpressao, false, true, titulo, oFonte, Color.DarkBlue, true, empresa);
            _dataGridViewPrinter.Detalhamento = detalhamento;

            return true;
        }

        /// <summary>
        /// Imprime os dados deste DataGridViewGrid em modo Retrato
        /// </summary>
        /// <param name="titulo">T�tulo que ser� impresso</param>
        /// <returns>True caso sucesso, ou False caso falhe.</returns>
        public bool Imprimir(string titulo)
        {
            return Imprimir(titulo, false, "");
        }

        /// <summary>
        /// Imprime os dados deste DataGridViewGrid com a op��o para selecionar Paisagem
        /// </summary>
        /// <param name="titulo">T�tulo que ser� impresso</param>
        /// <param name="paisagem">True para paisagem, ou False para retrato</param>
        /// <param name="empresa">Nome do estabelecimento do cliente</param>
        /// <returns></returns>
        public bool Imprimir(string titulo, bool paisagem, string empresa)
        {
            return Imprimir(titulo, paisagem, empresa, "", null);
        }

        public bool Imprimir(string titulo, bool paisagem, string empresa, string detalhamento)
        {
            return Imprimir(titulo, paisagem, empresa, detalhamento, null);
        }

        public bool Imprimir(string titulo, bool paisagem, string empresa, string detalhamento, PrinterSettings configuracaoImpressao)
        {
            if (ConfigurarImpressao(titulo, paisagem, empresa, detalhamento, configuracaoImpressao))
            {
                using (PrintPreviewDialog _caixaDialogoPreVisualizacao = new PrintPreviewDialog())
                {
                    _caixaDialogoPreVisualizacao.Document = _documentoImpressao;
                    ((Form)_caixaDialogoPreVisualizacao).WindowState = FormWindowState.Maximized;
                    _caixaDialogoPreVisualizacao.ShowDialog();
                }
            }
            return true;
        }

        /// <summary>
        /// Soma todos os valores num�ricos da coluna passada por par�metro.
        /// </summary>
        /// <param name="coluna">Indica a coluna que ser� somada</param>
        /// <returns>Retorna a soma da coluna solicitada</returns>
        public decimal ObterSomaColunaDecimal(int coluna)
        {
            decimal soma = 0;

            for (int i = 0; i < this.Rows.Count; i++)
            {
                string texto = ObterTextMatrix(i, coluna);
                texto = texto.Replace(".", "");

                if (tele.TiposDados.Texto.TestarNumerico(texto) == true)
                    soma += Convert.ToDecimal(texto);
            }
            return soma;

        }

        public decimal ObterSomaLinhaDecimal(int linha)
        {
            decimal soma = 0;

            for (int i = 0; i < this.Columns.Count; i++)
            {
                string texto = ObterTextMatrix(linha, i);
                texto = texto.Replace(".", "");

                if (tele.TiposDados.Texto.TestarNumerico(texto) == true)
                    soma += Convert.ToDecimal(texto);
            }
            return soma;

        }


        /// <summary>
        /// Soma todos os valores inteiros da coluna passada por par�metro.
        /// </summary>
        /// <param name="coluna">Indica a coluna que ser� somada</param>
        /// <returns>Retorna a soma da coluna solicitada</returns>
        public int ObterSomaColunaInt(int coluna)
        {
            int soma = 0;

            for (int i = 0; i < this.Rows.Count; i++)
                if (tele.TiposDados.Texto.TestarInteiro(ObterTextMatrix(i, coluna)) == true)
                    soma += Convert.ToInt32(ObterTextMatrix(i, coluna));

            return soma;
        }

        /// <summary>
        /// Linca com o devido procedimentos, apenas para facilitar a sintaxe.
        /// </summary>
        /// <param name="linha">Linha da c�lula</param>
        /// <param name="coluna">Coluna da c�lula</param>
        /// <returns>Valor encontrado na c�lula</returns>
        public string ObterTextMatrix(int linha, int coluna)
        {
            if (this.Rows[linha].Cells[coluna].Value == null)
                this.Rows[linha].Cells[coluna].Value = "";

            return this.Rows[linha].Cells[coluna].Value.ToString();
        }


        /// <summary>
        /// Passa o foco para uma determinada c�lula.
        /// </summary>
        /// <param name="linha">Linha que ir� receber o foco.</param>
        /// <param name="coluna">Coluna que ir� receber o foco.</param>
        public void SelecionarCelula(int linha, int coluna)
        {
            if (linha > -1)
            {
                this.CurrentCell.Selected = false;
                this.Rows[linha].Cells[coluna].Selected = true;
                if (this.Rows[linha].Cells[coluna].Visible)
                    this.CurrentCell = this.Rows[linha].Cells[coluna];
            }
        }

        /// <summary>
        /// Procura um texto e retorna a sua linha
        /// </summary>
        /// <param name="coluna">Coluna onde o texto ser� procurado</param>
        /// <param name="texto">Texto a ser procurado</param>
        /// <returns>N�mero da linha onde encontra-se o texto, ou -1 caso n�o encontre.</returns>
        public int ProcurarLinha(int coluna, string texto)
        {
            for (int i = 0; i < Rows.Count; i++)
                if (ObterTextMatrix(i, coluna) == texto)
                    return i;

            return -1;
        }

        /// <summary>
        /// Procura um texto no cabe�alho e retorna a sua coluna
        /// </summary>
        /// <param name="texto">Texto a ser procurado.</param>
        /// <returns>N�mero da coluna onde o texto foi encontrado, ou -1 caso n�o encontre.</returns>
        public int ProcurarColuna(string texto)
        {
            for (int i = 0; i < this.Columns.Count; i++)
                if (this.Columns[i].HeaderText == texto)
                    return i;

            return -1;
        }

        /// <summary>
        /// Percorre todas as c�lulas vis�veis do grid substituindo os dados conforme os par�metros.
        /// </summary>
        /// <param name="origem">Texto que deve ser substituido.</param>
        /// <param name="destino">Novo texto que ser� atribu�do � celula.</param>
        public void SubstutuirTodos(string origem, string destino)
        {
            for (int linha = 0; linha < this.Rows.Count; linha++)
                for (int coluna = 0; coluna < this.Columns.Count; coluna++)
                    if (ObterTextMatrix(linha, coluna) == origem)
                        if (this.Columns[coluna].Visible == true)
                            if (this.Rows[linha].Visible == true)
                                DefinirTextMatrix(linha, coluna, destino);

        }

        private void _documentoImpressao_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool continuar = _dataGridViewPrinter.DesenharDataGridView(e.Graphics, _logotipo);
            if (continuar == true)
                e.HasMorePages = true;
        }

        /// <summary>
        /// Deve ser usado para for�ar que a ordem original dos dados seja mantida
        /// </summary>
        public void DesabilitarReOrdenacaoDasColunas()
        {
            foreach (DataGridViewColumn cColuna in this.Columns)
                cColuna.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        /// <summary>
        /// Agrupa diversar funcionalidades de formata��o de colunas, inclusive deixar invis�vel quanda a largura da coluna for zero.
        /// </summary>
        /// <param name="coluna"></param>
        /// <param name="texto"></param>
        /// <param name="largura"></param>
        /// <param name="tipoEdicao"></param>
        /// <param name="decimais">Usado para uma coluna que tenha tipo de dados moeda.</param>
        public void FormatarColuna(int coluna, string texto, int largura, Classes.ItemComboBusca.TipoDados tipoEdicao, int decimais)
        {
            if (_tipoColunas == null)
                _tipoColunas = new Dictionary<int, Classes.ItemComboBusca.TipoDados>();

            this.Columns[coluna].HeaderText = texto;
            this.Columns[coluna].Width = largura;

            if (_tipoColunas.ContainsKey(coluna))
                _tipoColunas.Remove(coluna);
            _tipoColunas.Add(coluna, tipoEdicao);

            if (largura == 0)
                this.Columns[coluna].Visible = false;

            if ((tipoEdicao == Classes.ItemComboBusca.TipoDados.Moeda))
            {
                this.Columns[coluna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                this.Columns[coluna].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

        }

        /// <summary>
        /// Marca uma c�lula de uma coluna do tipo Check Box
        /// </summary>
        /// <param name="linha">Linha da c�lula</param>
        /// <param name="coluna">Coluna da c�lula</param>
        public void MarcarCelula(int linha, int coluna)
        {
            DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)Rows[linha].Cells[coluna];
            cbc.Value = GRID_CHECKED;
        }


        public void MarcarColuna(int coluna, bool valor)
        {
            foreach (DataGridViewRow linha in this.Rows)
            {
                if (valor)
                    this.MarcarCelula(linha.Index, coluna);
                else
                    this.DesmarcarCelula(linha.Index, coluna);
            }
        }

        /// <summary>
        /// Testa se uma c�lula est� marcada. C�lula de uma coluna do tipo Check Box.
        /// </summary>
        /// <param name="linha">Linha da c�lula</param>
        /// <param name="coluna">Coluna da c�lula</param>
        /// <returns>True caso esteja marcada, ou False caso esteja desmarcada</returns>
        public bool TestarCelulaMarcada(int linha, int coluna)
        {
            DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)Rows[linha].Cells[coluna];
            if (cbc.Value == null || cbc.Value == "")
                return false;
            if (cbc.Value.ToString() == "True")
                cbc.Value = GRID_CHECKED;
            else if (cbc.Value.ToString() == "False")
                cbc.Value = GRID_UNCHECKED;

            if (Convert.ToInt32(cbc.Value) == GRID_CHECKED)
                return true;

            return false;
        }

        /// <summary>
        /// Desmarca uma c�lula de uma coluna do tipo Check Box
        /// </summary>
        /// <param name="linha">Linha da c�lula</param>
        /// <param name="coluna">Coluna da c�lula</param>
        public void DesmarcarCelula(int linha, int coluna)
        {
            DataGridViewCheckBoxCell cbc = (DataGridViewCheckBoxCell)Rows[linha].Cells[coluna];
            cbc.Value = GRID_UNCHECKED;
        }

        /// <summary>
        /// Retorna um objeto do tipo Coluna Check Box para uso gen�rico
        /// </summary>
        /// <returns>Retorna um objeto do tipo Coluna Check Box</returns>
        public DataGridViewCheckBoxColumn RetornarColunaCheckBox()
        {
            DataGridViewCheckBoxColumn colunaCheckBox = new DataGridViewCheckBoxColumn();
            colunaCheckBox.Width = 20;
            colunaCheckBox.DataPropertyName = "ColChk" + Columns.Count.ToString();
            colunaCheckBox.HeaderText = " ";
            colunaCheckBox.Name = "ColChk" + Columns.Count.ToString();
            colunaCheckBox.ValueType = typeof(string);
            colunaCheckBox.Visible = true;
            colunaCheckBox.TrueValue = GRID_CHECKED;
            colunaCheckBox.FalseValue = GRID_UNCHECKED;

            return colunaCheckBox;
        }

        public DataGridViewImageColumn RetornarColunaImagem()
        {
            DataGridViewImageColumn colunaImagem = new DataGridViewImageColumn();
            colunaImagem.Width = 20;
            colunaImagem.DataPropertyName = "ColImg" + Columns.Count.ToString();
            colunaImagem.HeaderText = " ";
            colunaImagem.Name = "ColImg" + Columns.Count.ToString();
            colunaImagem.ValueType = typeof(string);
            colunaImagem.Visible = true;

            return colunaImagem;
        }

        /// <summary>
        /// Usado para executar formata��es como desativar a exclus�o das c�lulas, altura das linhas e definir o n�mero de colunas
        /// </summary>
        /// <param name="nroColunas">N�mero de colunas que deve ser definido</param>
        public void ExecutarFormatacaoPadrao(int nroColunas)
        {
            base.AllowUserToResizeRows = false;
            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = false;
            base.Columns.Clear();
            //base.ColumnCount = nroColunas;

            for (int aux = 0; aux < nroColunas; aux++)
            {
                var col = new DataGridViewTextBoxColumnEx();
                this.Columns.Add(col);
            }

        }

        /// <summary>
        /// Usado para preenche o grid com uma lista de colunas, a largura vai ser dividida proporcionalmente
        /// </summary>
        /// <param name="colunas"></param>
        public void PreencherColunasGrid(List<int> colunas)
        {
            var resto = 20;
            foreach (DataGridViewColumn coluna in this.Columns)
            {
                if (colunas.Contains(coluna.Index))
                    continue;
                resto += coluna.Width;
            }
            resto = (int)((this.Width - resto) / colunas.Count);
            foreach (var coluna in colunas)
            {
                this.Columns[coluna].Width = resto;
            }

        }

        /// <summary>
        /// Agrupa diversar funcionalidades de formata��o de colunas, inclusive deixar invis�vel quanda a largura da coluna for zero.
        /// </summary>
        /// <param name="coluna">Coluna a ser formatada</param>
        /// <param name="texto">Texto que ser� exibido no cabe�alho</param>
        /// <param name="largura">Largura da coluna</param>
        /// <param name="tipoEdicao">Tipo de edi��o desta coluna</param>
        public void FormatarColuna(int coluna, string texto, int largura, Classes.ItemComboBusca.TipoDados tipoEdicao)
        {
            FormatarColuna(coluna, texto, largura, tipoEdicao, 2);
        }

        /// <summary>
        /// Agrupa diversar funcionalidades de formata��o de colunas, inclusive deixar invis�vel quanda a largura da coluna for zero.
        /// </summary>
        /// <param name="coluna">Coluna a ser formatada</param>
        /// <param name="texto">Texto que ser� exibido no cabe�alho</param>
        /// <param name="largura">Tipo de edi��o desta coluna</param>
        public void FormatarColuna(int coluna, string texto, int largura)
        {
            FormatarColuna(coluna, texto, largura, Classes.ItemComboBusca.TipoDados.NaoEditavel);
        }

        public void FormatarColuna(int coluna, string texto, int largura, DataGridViewContentAlignment alinhamento)
        {
            FormatarColuna(coluna, texto, largura, Classes.ItemComboBusca.TipoDados.NaoEditavel);
            Columns[coluna].DefaultCellStyle.Alignment = alinhamento;
            Columns[coluna].HeaderCell.Style.Alignment = alinhamento;
        }


        /// <summary>
        /// Agrupa as rotinas de altera��o de cor de fundo e fonte de uma determinada c�lula.
        /// </summary>
        /// <param name="linha"></param>
        /// <param name="coluna"></param>
        /// <param name="corFundo"></param>
        /// <param name="corFonte"></param>
        public void ColorirCelula(int linha, int coluna, Color corFundo, Color corFonte)
        {
            this.Rows[linha].Cells[coluna].Style.BackColor = corFundo;
            this.Rows[linha].Cells[coluna].Style.ForeColor = corFonte;

        }

        /// <summary>
        /// Percorre as linhas do grid deixando-as cada uma com uma cor.
        /// </summary>
        /// <param name="corFundo">Cor de fundo que ser� alternada com o branco.</param>
        /// <param name="corFonte">Cor da fonte quando a cor de fundo for igual � var�vel corFundo.</param>
        public void AlternarCores(Color corFundo, Color corFonte)
        {
            bool branco = true;

            for (int linha = 0; linha < this.Rows.Count; linha++)
            {
                if (branco == false)
                    ColorirLinha(linha, corFundo, corFonte);

                branco = !branco;
            }
        }

        /// <summary>
        /// Percorre as linhas do grid deixando-as cada uma com uma cor.
        /// </summary>
        /// <param name="corFundo">Cor de fundo que ser� alternada com o branco.</param>
        public void AlternarCores(Color corFundo)
        {
            AlternarCores(corFundo, SystemColors.WindowText);
        }

        /// <summary>
        /// Percorre as c�lulas de uma determinada linha, chamando o procedimento ColorirCelula.
        /// </summary>
        /// <param name="linha">Linha que ser� colorida</param>
        /// <param name="corFundo">Cor de fundo das c�lulas</param>
        /// <param name="corFonte">Cor da fonte das c�lulas</param>
        public void ColorirLinha(int linha, Color corFundo, Color corFonte)
        {
            int coluna;

            for (coluna = 0; coluna < this.Columns.Count; coluna++)
                ColorirCelula(linha, coluna, corFundo, corFonte);

        }

        /// <summary>
        /// Percorre as c�lulas de uma determinada coluna, chamando o procedimento ColorirCelula.
        /// </summary>
        /// <param name="coluna">Coluna a ser colorida</param>
        /// <param name="corFundo">Cor de fundo das c�lulas</param>
        /// <param name="corFonte">Cor da fonte das c�lulas</param>
        public void ColorirColuna(int coluna, Color corFundo, Color corFonte)
        {
            int linha;

            for (linha = 0; linha < this.Rows.Count; linha++)
                ColorirCelula(linha, coluna, corFundo, corFonte);

        }

        /// <summary>
        /// Linca com o devido procedimentos, apenas para facilitar a sintaxe.
        /// </summary>
        /// <param name="linha">Linha que receber� o valor</param>
        /// <param name="coluna">Coluna que receber� o valor</param>
        /// <param name="texto">Texto que ser� atribu�do</param>
        public void DefinirTextMatrix(int linha, int coluna, string texto)
        {
            this.Rows[linha].Cells[coluna].Value = texto;
        }

        /// <summary>
        /// Linca com o devido procedimentos, apenas para facilitar a sintaxe.
        /// </summary>
        /// <param name="linha">Linha que receber� o valor</param>
        /// <param name="coluna">Coluna que receber� o valor</param>
        /// <param name="qtdColunas">Quantidades de colunas em que ser� feito merge</param>
        public void JuntarColunas(int linha, int coluna, int qtdColunas)
        {

            var cell = (DataGridViewTextBoxCellEx)this.Rows[linha].Cells[coluna];
            cell.ColumnSpan = qtdColunas;

        }

        /// <summary>
        /// Linca com o devido procedimentos, apenas para facilitar a sintaxe.
        /// </summary>
        /// <param name="linha">Linha que receber� o valor</param>
        /// <param name="coluna">Coluna que receber� o valor</param>
        /// <param name="qtdColunas">Quantidades de linhas em que ser� feito merge</param>
        public void JuntarLinhas(int linha, int coluna, int qtdLinhas)
        {

            var cell = (DataGridViewTextBoxCellEx)this.Rows[linha].Cells[coluna];
            cell.RowSpan = qtdLinhas;

        }

        /// <summary>
        /// Linca com o devido procedimentos, apenas para facilitar a sintaxe (pegando sempre a ultima linha).
        /// </summary>
        /// <param name="coluna">Coluna que receber� o valor</param>
        /// <param name="texto">Texto que ser� atribu�do</param>
        public void DefinirTextMatrixUltimaLinha(int coluna, string texto)
        {
            DefinirTextMatrix(this.Rows.Count - 1, coluna, texto);
        }

        /// <summary>
        /// Se a coluna n�o for edit�vel, cancela a edi��o, se for edit�vel, guarda o valor da c�lula antes da edi��o.
        /// Como padr�o, nenhuma coluna � edit�vel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewPersonalizado_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (_tipoColunas.ContainsKey(e.ColumnIndex) == false)
                e.Cancel = true;
            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.NaoEditavel)
                e.Cancel = true;
            else
                _ultimoValorCelula = ObterTextMatrix(e.RowIndex, e.ColumnIndex);
        }

        private void DataGridViewPersonalizado_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string valorAtual;

            if (this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";

            valorAtual = this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Inteiro)
            {
                //Se o valor nao for inteiro, volta ao valor anterior
                if (tele.TiposDados.Texto.TestarInteiro(valorAtual) == false)
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, _ultimoValorCelula);
            }

            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Moeda)
            {
                //Se o valor nao for moeda volta ao valor anterior, se for moeda formata com o numero de decimais correto

                if (valorAtual.StartsWith(","))
                    valorAtual = "0" + valorAtual;
                if (valorAtual.EndsWith(","))
                    valorAtual = valorAtual.Substring(0, (int)valorAtual.Length - 1);
                if (tele.TiposDados.Texto.TestarNumerico(valorAtual) == false)
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, _ultimoValorCelula);
                else
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, tele.TiposDados.Texto.FormatarMoeda(valorAtual, _decimais));
            }
            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Data)
            {
                string data = Telecon.Genericos.Controles.CaixaTextoData.SugerirData(this[e.RowIndex, e.ColumnIndex]);

                if (data == "")
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, _ultimoValorCelula);
                else
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, data);
            }

            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.HoraMinuto)
            {

                if (tele.TiposDados.Texto.TestarInteiro(valorAtual) == true && valorAtual.ToString().Length == 4)
                    valorAtual = tele.TiposDados.Texto.Left(valorAtual, 2) + ":" + tele.TiposDados.Texto.Right(valorAtual, 2);

                if (tele.TiposDados.Data.TestarHoraMinutos(valorAtual) == false)
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, _ultimoValorCelula);
                else
                    DefinirTextMatrix(e.RowIndex, e.ColumnIndex, valorAtual);
            }
            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Maiusculo)
            {
                DefinirTextMatrix(e.RowIndex, e.ColumnIndex, valorAtual.ToUpper());
            }
            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Minusculo)
            {
                DefinirTextMatrix(e.RowIndex, e.ColumnIndex, valorAtual.ToLower());
            }
            else if (_tipoColunas[e.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Livre)
            {
                //N�o faz nada
            }
        }

        /// <summary>
        /// Retorna o n�mero das c�lulas checadas em uma coluna
        /// </summary>
        /// <param name="coluna">Coluna que ser� percorrida</param>
        /// <returns>Retorna o n�mero das c�lulas checadas em uma coluna</returns>
        public int ContarCelulasChecadas(int coluna)
        {
            int contador = 0;

            for (int i = 0; i < Rows.Count; i++)
                if (TestarCelulaMarcada(i, coluna))
                    contador++;

            return contador;
        }

        private void DataGridViewPersonalizado_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.CurrentRow == null)
                return;

            if (_tipoColunas[this.CurrentCell.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Inteiro || _tipoColunas[this.CurrentCell.ColumnIndex] == Classes.ItemComboBusca.TipoDados.Moeda)
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    SendKeys.Send("0");
                    SendKeys.Send("{ENTER}");
                }
            }
        }
        #endregion

        private void DataGridViewPersonalizado_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == -1)
                return;
            if (_tipoColunas[e.ColumnIndex] == Telecon.Genericos.Controles.Classes.ItemComboBusca.TipoDados.CheckBox &&
                e.RowIndex > -1)
                this.EndEdit();
        }

        /// <summary>
        /// Desativa a ordena��o de todas as colunas no clique do usu�rio
        /// </summary>
        public void DesativarOrdenacaoColunas()
        {
            for (int i = 0; i < this.Columns.Count - 1; i++)
                this.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        public DataGridViewImageCell ObterImagemLinha(int linha, int coluna)
        {
            return (DataGridViewImageCell)Rows[linha].Cells[coluna];
        }

        public void DefinirImagem(int coluna, Bitmap imagem)
        {
            DefinirImagem(UltimaLinha.Index, coluna, imagem);
        }

        public void DefinirImagem(int linha, int coluna, Bitmap imagem)
        {
            var img = ObterImagemLinha(linha, coluna);
            img.Value = imagem;
        }

        public void DefinirLinhaNegrito(int linha)
        {
            for (var c = 0; c < Columns.Count; c++)
                DefinirCelulaNegrito(linha, c);
        }

        private void DataGridViewPersonalizado_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_tipoColunas[e.ColumnIndex] == ItemComboBusca.TipoDados.CheckBox && Rows.Count > 0 && _marcarCelulasCliqueCabecalho)
            {
                var marcar = !TestarCelulaMarcada(0, e.ColumnIndex);
                MarcarColuna(e.ColumnIndex, marcar);

                //BUG DO GRID QUE ESTAVA MOSTRANDO A 1� CHECKBOX(A COM O FOCO) COMO DESMARCADA MESMO ESTANDO MARCADA
                if (e.ColumnIndex == 0)
                    SelecionarCelula(0, 1);
                else
                    SelecionarCelula(0, e.ColumnIndex - 1);
                this.Visible = !this.Visible;
                this.Visible = !this.Visible;



            }
        }

        public void DuplicarLinha(int linhaOrigem)
        {
            Rows.Add();
            for (var c = 0; c < Columns.Count; c++)
                this[c] = this[linhaOrigem, c];
        }

        public void FormatarColunaImagem(int coluna)
        {
            FormatarColuna(coluna, "", 50);

            Columns.RemoveAt(coluna);
            Columns.Insert(coluna, RetornarColunaImagem());
        }

        public void FormatarColunaCheckbox(int coluna)
        {
            Columns.RemoveAt(coluna);
            Columns.Insert(coluna, RetornarColunaCheckBox());
            DefinirTipoColuna(coluna, ItemComboBusca.TipoDados.CheckBox);
        }
        public void MoverLinha(int origem, int destino)
        {
            var item = (DataGridViewRow)Rows[origem].Clone();
            foreach (DataGridViewColumn coluna in Columns)
            {
                //item.Cells[coluna.Index] = Rows[origem].Cells[coluna.Index].Clone();
                var propriedades = item.Cells[coluna.Index].GetType().GetProperties();
                foreach (var prop in propriedades)
                {
                    if (!prop.CanWrite)
                        continue;

                    try
                    {
                        var valor = prop.GetValue(Rows[origem].Cells[coluna.Index], null);
                        prop.SetValue(item.Cells[coluna.Index], valor, null);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                }
            }
            Rows.RemoveAt(origem);
            Rows.Insert(destino, item);
        }
        public void MoverParaBaixo(int indice, bool selecionar)
        {
            if (indice < Rows.Count - 1)
            {
                MoverLinha(indice, indice + 1);
                if (selecionar)
                {
                    Rows[indice + 1].Selected = true;
                }
            }
        }

        public void MoverParaCima(int indice, bool selecionar)
        {
            if (indice > 0)
            {
                MoverLinha(indice, indice - 1);
                if (selecionar)
                {
                    Rows[indice - 1].Selected = true;
                }
            }
        }


        public void Adicionar()
        {
            Rows.Add();
        }

        public void RemoveAutoSizeDinamico()
        {
            for (int i = 0; i < this.Columns.Count; i++)
                this.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
        }

        public void AdicionarAutoSizeDinamico(int indexColumnFill)
        {
            for (int i = 0; i < this.Columns.Count; i++)
                this.Columns[i].AutoSizeMode = i != indexColumnFill ? DataGridViewAutoSizeColumnMode.AllCells : DataGridViewAutoSizeColumnMode.Fill;
        }
    }
}
