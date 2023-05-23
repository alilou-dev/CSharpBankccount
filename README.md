# CSharpBankccount
app with c# for manage bank Account as a test for ExaltIT
# requirement : VS or VSCODE (éditeur de code source)
# How to start project 
  # clone the repo first : git clone https://github.com/alilou-dev/CSharpBankccount/tree/master
  
# API : 
  - Create a new account - HttpPost("/account/post") - body : label (string)
  - Get All Account - HttpGet("/account") 
  - Get Account Info - HttpGet("/account/{accountRef}") - body : none
  - Check Account Balance - HttpGet("/account/{accountRef}/balance") - body : none
  - Debit Or Credit Account HttpPost("/account/{accountRef}/transaction") -  body : { string TransactionType ("d" for debit transaction and "c" for credit one), float Ammount, string Label (not requried) }
  - View all transactin - HttpGet("/account/{accountRef}/transactions") - body : none
  
