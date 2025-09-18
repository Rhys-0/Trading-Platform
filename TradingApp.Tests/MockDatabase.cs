using Dapper;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Testcontainers.PostgreSql;

namespace TradingApp.Tests {
    public class MockDatabase : IAsyncLifetime {
        private readonly PostgreSqlContainer _postgres;

        public string ConnectionString => _postgres.GetConnectionString();

        public MockDatabase() {
            _postgres = new PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("password")
                .WithImage("postgres:15-alpine")
                .Build();
        }

        public async Task InitializeAsync() {
            await _postgres.StartAsync();

            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Create database schema
            await connection.ExecuteAsync(@"
                CREATE TABLE public.users (
                  user_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
                  username text NOT NULL UNIQUE,
                  email text NOT NULL UNIQUE,
                  password_hash text NOT NULL,
                  first_name text NOT NULL,
                  last_name text NOT NULL,
                  starting_cash_balance numeric NOT NULL,
                  current_cash_balance numeric NOT NULL,
                  CONSTRAINT users_pkey PRIMARY KEY (user_id)
                );
                CREATE TABLE public.portfolio (
                  portfolio_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
                  user_id bigint NOT NULL,
                  value numeric NOT NULL,
                  net_profit numeric NOT NULL,
                  percentage_return numeric NOT NULL,
                  CONSTRAINT portfolio_pkey PRIMARY KEY (portfolio_id),
                  CONSTRAINT portfolio_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id)
                );
                CREATE TABLE public.position (
                  position_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
                  portfolio_id bigint NOT NULL,
                  stock_symbol text NOT NULL,
                  total_quantity int4 NOT NULL,
                  CONSTRAINT position_pkey PRIMARY KEY (position_id),
                  CONSTRAINT position_portfolio_id_fkey FOREIGN KEY (portfolio_id) REFERENCES public.portfolio(portfolio_id)
                );
                CREATE TABLE public.trade (
                  trade_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
                  user_id bigint NOT NULL,
                  trade_type text NOT NULL,
                  stock_symbol text NOT NULL,
                  quantity int4 NOT NULL,
                  price numeric NOT NULL,
                  time timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'::text),
                  CONSTRAINT trade_pkey PRIMARY KEY (trade_id),
                  CONSTRAINT TRADE_user_id_fkey FOREIGN KEY (user_id) REFERENCES public.users(user_id)
                );
                CREATE TABLE public.purchase_lot (
                  purchase_lot_id bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
                  position_id bigint NOT NULL,
                  trade_id bigint NOT NULL,
                  quantity int4 NOT NULL,
                  purchase_price numeric NOT NULL,
                  purchase_date timestamp with time zone NOT NULL DEFAULT (now() AT TIME ZONE 'utc'::text),
                  CONSTRAINT purchase_lot_pkey PRIMARY KEY (purchase_lot_id),
                  CONSTRAINT purchase_lot_trade_id_fkey FOREIGN KEY (trade_id) REFERENCES public.trade(trade_id),
                  CONSTRAINT purchase_lot_position_id_fkey FOREIGN KEY (position_id) REFERENCES public.position(position_id)
                );");
        }

        public async Task DisposeAsync() => await _postgres.DisposeAsync();

        public async Task ResetDatabaseAsync() {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            // Drop rows
            await connection.ExecuteAsync(@"
                DELETE FROM public.purchase_lot;
                DELETE FROM public.position;
                DELETE FROM public.portfolio;
                DELETE FROM public.trade;
                DELETE FROM public.users;");
        }

        public async Task ExecuteAsync(string sql, object? param = null) {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, param);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null) {
            using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param);
        }
    }
}
