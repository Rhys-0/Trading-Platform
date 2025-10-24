# Mock Trading Platform

## Features
- User portfolio dashboard
- Real-time stock price display
- Buy / sell order page
- Transaction history dashboard
- User account creation page, login page and home page
- User leaderboard
- Administrator dashboard
- Database setup
- Stock News Page
- Background processes

### User Portfolio Dashboard
**Responsible Student:** Enoch \
**Model Files:** HoldingsManager.cs\
**View / ViewModel Files:** UserDashboard.razor\
**Other Files:** N/A

### Real-time stock price display
**Responsible Student:** Hoang \
**Model Files:** Stock.cs, Stocks.cs \
**View / ViewModel Files:** StockPanel.razor (Buy/Sell features are made by Leo), StockQuote.razor, Stocks.razor \
**Other Files:** tradingview.js, tradingviewBridge.js, tvLoader.js

### Buy / sell order page
**Responsible Student:** Leo \
**Model Files:**  \
**View / ViewModel Files:** TradingModal.razor, TradeNotification.razor, Stoc ksPanel.razor (Buy/sell Button) \
**Other Files:** (Interacts with following models) Trade.cs, Positions.cs, Purchaselot.cs, PortfolioService.cs

### Transaction history dashboard
**Responsible Student:** Hoang \
**Model Files:** Trade.cs \
**View / ViewModel Files:** Trades.razor \
**Other Files:** ITradeManager.cs, TradeManager.cs, download.js 

### User account creation page, login page and home page
**Responsible Student:** Shilpi \
**Model Files:** \ User.cs
**View / ViewModel Files:** \ Home.razor, Login.razor, Register.razor
**Other Files:** AuthenticationService.cs, UserService.cs, UserManager.cs, LoginManager.cs, PasswordHasher.cs, AuthenticationTests.cs

### User leaderboard
**Responsible Student:** Shilpi \
**Model Files:** \ LeaderboardEntry.cs
**View / ViewModel Files:** \ Leaderboard.razor
**Other Files:** ILeaderboardService.cs, LeaderboardService.cs, LeaderboardTests.cs

### Administrator dashboard
**Responsible Student:** Enoch \
**Model Files:** \
**View / ViewModel Files:** Userlist.Razor\
**Other Files:** GetAllUsers & GetUserById in UserManager

### Database setup
**Responsible Student:** Rhys \
**Model Files:** Portfolio.cs, Position.cs, PurchaseLot.cs, Trade.cs, User.cs \
**View / ViewModel Files:** N/A \
**Other Files:** DatabaseConnection.cs, LoginManager.cs, UserManager.cs, LoginService.cs, UserService.cs

### Stock News Page
**Responsible Student:** Leo \
**Model Files:** NewsService.cs, \
**View / ViewModel Files:** StockNews.razor, NewsCard.razor\
**Other Files:**

### Background Processes
**Responsible Student:** Rhys \
**Model Files:** Stock.cs, Stocks.cs \
**View / ViewModel Files:** N/A \
**Other Files:** FinnhubParser.cs (made by Hoang), StockPriceService.cs
