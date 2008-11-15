#pragma once

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Windows::Forms;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Threading;


namespace EraserCtxMenu {

	/// <summary>
	/// Summary for SecureMove
	///
	/// WARNING: If you change the name of this class, you will need to change the
	///          'Resource File Name' property for the managed resource compiler tool
	///          associated with all .resx files this class depends on.  Otherwise,
	///          the designers will not be able to interact properly with localized
	///          resources associated with this form.
	/// </summary>
	public ref class SecureMove : public System::Windows::Forms::Form
	{
	public:
		SecureMove(void)
		{
			InitializeComponent();
			//
			//TODO: Add the constructor code here
			//
		}

	protected:
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		~SecureMove()
		{
			if (components)
			{
				delete components;
			}
		}
	private: System::Windows::Forms::ProgressBar^  moveProgressBar;
	protected: 

	protected: 
	private: System::Windows::Forms::Button^  cancel;
	private: System::Windows::Forms::PictureBox^  pictureBox1;
	private: System::Windows::Forms::Label^  fromLabel;
	private: System::Windows::Forms::Label^  toLabel;
	private: System::Windows::Forms::TextBox^  textBoxFrom;
	private: System::Windows::Forms::TextBox^  textBoxTo;
	private: System::Windows::Forms::Button^  openBrowserBtn;
	private: System::Windows::Forms::Button^  saveBrowserBtn;
	private: System::Windows::Forms::OpenFileDialog^  openFileDialog1;
	private: System::Windows::Forms::SaveFileDialog^  saveFileDialog1;
	private: System::Windows::Forms::Button^  start;

	private:
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent(void)
		{
			this->moveProgressBar = (gcnew System::Windows::Forms::ProgressBar());
			this->cancel = (gcnew System::Windows::Forms::Button());
			this->pictureBox1 = (gcnew System::Windows::Forms::PictureBox());
			this->fromLabel = (gcnew System::Windows::Forms::Label());
			this->toLabel = (gcnew System::Windows::Forms::Label());
			this->textBoxFrom = (gcnew System::Windows::Forms::TextBox());
			this->textBoxTo = (gcnew System::Windows::Forms::TextBox());
			this->openBrowserBtn = (gcnew System::Windows::Forms::Button());
			this->saveBrowserBtn = (gcnew System::Windows::Forms::Button());
			this->openFileDialog1 = (gcnew System::Windows::Forms::OpenFileDialog());
			this->saveFileDialog1 = (gcnew System::Windows::Forms::SaveFileDialog());
			this->start = (gcnew System::Windows::Forms::Button());
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->pictureBox1))->BeginInit();
			this->SuspendLayout();
			// 
			// moveProgressBar
			// 
			this->moveProgressBar->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->moveProgressBar->Location = System::Drawing::Point(12, 145);
			this->moveProgressBar->Name = L"moveProgressBar";
			this->moveProgressBar->Size = System::Drawing::Size(270, 20);
			this->moveProgressBar->TabIndex = 0;
			// 
			// cancel
			// 
			this->cancel->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Bottom | System::Windows::Forms::AnchorStyles::Right));
			this->cancel->Location = System::Drawing::Point(369, 143);
			this->cancel->Name = L"cancel";
			this->cancel->Size = System::Drawing::Size(75, 23);
			this->cancel->TabIndex = 1;
			this->cancel->Text = L"&Cancel";
			this->cancel->UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this->pictureBox1->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Bottom) 
				| System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->pictureBox1->Location = System::Drawing::Point(12, 12);
			this->pictureBox1->Name = L"pictureBox1";
			this->pictureBox1->Size = System::Drawing::Size(130, 125);
			this->pictureBox1->TabIndex = 2;
			this->pictureBox1->TabStop = false;
			// 
			// fromLabel
			// 
			this->fromLabel->AutoSize = true;
			this->fromLabel->Location = System::Drawing::Point(172, 12);
			this->fromLabel->Name = L"fromLabel";
			this->fromLabel->Size = System::Drawing::Size(60, 13);
			this->fromLabel->TabIndex = 4;
			this->fromLabel->Text = L"Move from:";
			// 
			// toLabel
			// 
			this->toLabel->AutoSize = true;
			this->toLabel->Location = System::Drawing::Point(172, 61);
			this->toLabel->Name = L"toLabel";
			this->toLabel->Size = System::Drawing::Size(49, 13);
			this->toLabel->TabIndex = 5;
			this->toLabel->Text = L"Move to:";
			// 
			// textBoxFrom
			// 
			this->textBoxFrom->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->textBoxFrom->Location = System::Drawing::Point(193, 28);
			this->textBoxFrom->Name = L"textBoxFrom";
			this->textBoxFrom->Size = System::Drawing::Size(210, 20);
			this->textBoxFrom->TabIndex = 6;
			// 
			// textBoxTo
			// 
			this->textBoxTo->Anchor = static_cast<System::Windows::Forms::AnchorStyles>(((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Left) 
				| System::Windows::Forms::AnchorStyles::Right));
			this->textBoxTo->Location = System::Drawing::Point(193, 77);
			this->textBoxTo->Name = L"textBoxTo";
			this->textBoxTo->Size = System::Drawing::Size(210, 20);
			this->textBoxTo->TabIndex = 7;
			// 
			// openBrowserBtn
			// 
			this->openBrowserBtn->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->openBrowserBtn->Location = System::Drawing::Point(409, 26);
			this->openBrowserBtn->Name = L"openBrowserBtn";
			this->openBrowserBtn->Size = System::Drawing::Size(35, 23);
			this->openBrowserBtn->TabIndex = 8;
			this->openBrowserBtn->Text = L"...";
			this->openBrowserBtn->UseVisualStyleBackColor = true;
			this->openBrowserBtn->Click += gcnew System::EventHandler(this, &SecureMove::openBrowserBtn_Click);
			// 
			// saveBrowserBtn
			// 
			this->saveBrowserBtn->Anchor = static_cast<System::Windows::Forms::AnchorStyles>((System::Windows::Forms::AnchorStyles::Top | System::Windows::Forms::AnchorStyles::Right));
			this->saveBrowserBtn->Location = System::Drawing::Point(409, 75);
			this->saveBrowserBtn->Name = L"saveBrowserBtn";
			this->saveBrowserBtn->Size = System::Drawing::Size(35, 23);
			this->saveBrowserBtn->TabIndex = 9;
			this->saveBrowserBtn->Text = L"...";
			this->saveBrowserBtn->UseVisualStyleBackColor = true;
			this->saveBrowserBtn->Click += gcnew System::EventHandler(this, &SecureMove::saveBrowserBtn_Click);
			// 
			// openFileDialog1
			// 
			this->openFileDialog1->FileName = L"openFileDialog1";
			// 
			// start
			// 
			this->start->Location = System::Drawing::Point(296, 147);
			this->start->Name = L"start";
			this->start->Size = System::Drawing::Size(75, 23);
			this->start->TabIndex = 10;
			this->start->Text = L"&Start";
			this->start->UseVisualStyleBackColor = true;
			this->start->Click += gcnew System::EventHandler(this, &SecureMove::start_Click);
			// 
			// SecureMove
			// 
			this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
			this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
			this->ClientSize = System::Drawing::Size(456, 178);
			this->Controls->Add(this->start);
			this->Controls->Add(this->saveBrowserBtn);
			this->Controls->Add(this->openBrowserBtn);
			this->Controls->Add(this->textBoxTo);
			this->Controls->Add(this->textBoxFrom);
			this->Controls->Add(this->toLabel);
			this->Controls->Add(this->fromLabel);
			this->Controls->Add(this->pictureBox1);
			this->Controls->Add(this->cancel);
			this->Controls->Add(this->moveProgressBar);
			this->Name = L"SecureMove";
			this->Text = L"SecureMove";
			(cli::safe_cast<System::ComponentModel::ISupportInitialize^  >(this->pictureBox1))->EndInit();
			this->ResumeLayout(false);
			this->PerformLayout();

		}
#pragma endregion

		// This method was supposed to be threaded
		// but threading is a nightmare on VC++ so just do a single threaded 
		// version.
	private: void Move(String ^dst, String ^src)
					 {
						 IO::FileStream ^sf, ^df;
						 int read = 0;
						 int progress = 0;

						 // here it is best to do the move manually
						 // as we have to hold locks to the both of
						 // the files simultaniously.
						 try
						 {
							 try
							 {
								 sf = gcnew IO::FileStream(src, IO::FileMode::Open, 
									 IO::FileAccess::Read,
									 IO::FileShare::None);

								 df = gcnew IO::FileStream(dst, IO::FileMode::CreateNew, 
									 IO::FileAccess::Write,
									 IO::FileShare::None);

								 do
								 {
									 array<Byte,1> ^buffer = gcnew array<Byte,1>(65536);
									 read = sf->Read(buffer, 0, buffer->Length);
									 df->Write(buffer, 0, read);
									 progress = (sf->Position * 100) / sf->Length;

									 moveProgressBar->Value = progress ;

								 } while((read != 0) && (sf->Position == sf->Length));

								 array<String^> ^src_array = gcnew array<String^>(1);
								 src_array[0] = src;

								 // EraseFiles( src_array );
							 } 
							 catch(IO::IOException ^ex)
							 {
								 // some error handling
							 }
						 } 
						 finally
						 {
							 sf->Close();
							 df->Close();
						 }
					 }
	private: System::Void openBrowserBtn_Click(System::Object^  sender, System::EventArgs^  e) 
					 {
						 if(openFileDialog1->ShowDialog() == Windows::Forms::DialogResult::OK)
						 {
							 textBoxFrom->Text = openFileDialog1->FileName;
						 }
					 }
	private: System::Void saveBrowserBtn_Click(System::Object^  sender, System::EventArgs^  e) 
					 {
						 if(saveFileDialog1->ShowDialog() == Windows::Forms::DialogResult::OK)
						 {
							 textBoxTo->Text = saveFileDialog1->FileName;
						 }
					 }

	private: System::Void updateUI()
					 {
						 if(state == RUNNING)
							 start->Text = L"&Pause";
						 else
							 start->Text = L"&Start";
					 }

	private: System::Void start_Click(System::Object^  sender, System::EventArgs^  e) 
					 {
						 if(state == RUNNING)
						 {
							 // pause
						 }
						 else if(state == NOT_RUNNING)
						 {
							 // start						 
						 }
						 updateUI();
					 }

	private: static const bool RUNNING = true;
	private: static const bool NOT_RUNNING  = false;
	private: bool state;
	private: System::Threading::Thread ^thread ;
	};
}
