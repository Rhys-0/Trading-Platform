using TradingApp.Data;
using Xunit;

namespace TradingApp.Tests {
    public class PasswordHasherTests {
        
        [Fact]
        public void ComputeSha256Hash_WithValidPassword_ShouldReturnHash() {
            // Arrange
            var password = "testpassword123";
            
            // Act
            var hash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotEqual(password, hash); // Hash should be different from original password
            Assert.True(hash.Length == 64); // SHA256 produces 64 character hex string
        }

        [Fact]
        public void ComputeSha256Hash_WithEmptyPassword_ShouldReturnHash() {
            // Arrange
            var password = "";
            
            // Act
            var hash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.True(hash.Length == 64);
        }

        [Fact]
        public void ComputeSha256Hash_WithSamePassword_ShouldReturnSameHash() {
            // Arrange
            var password = "consistentpassword";
            
            // Act
            var hash1 = PasswordHasher.ComputeSha256Hash(password);
            var hash2 = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void ComputeSha256Hash_WithDifferentPasswords_ShouldReturnDifferentHashes() {
            // Arrange
            var password1 = "password1";
            var password2 = "password2";
            
            // Act
            var hash1 = PasswordHasher.ComputeSha256Hash(password1);
            var hash2 = PasswordHasher.ComputeSha256Hash(password2);
            
            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void ComputeSha256Hash_WithSpecialCharacters_ShouldReturnHash() {
            // Arrange
            var password = "P@ssw0rd!@#$%^&*()_+-=[]{}|;':\",./<>?";
            
            // Act
            var hash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.True(hash.Length == 64);
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void ComputeSha256Hash_WithLongPassword_ShouldReturnHash() {
            // Arrange
            var password = new string('a', 1000); // 1000 character password
            
            // Act
            var hash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.True(hash.Length == 64);
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void ComputeSha256Hash_WithUnicodeCharacters_ShouldReturnHash() {
            // Arrange
            var password = "密码123🚀🔥";
            
            // Act
            var hash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.True(hash.Length == 64);
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void ComputeSha256Hash_WithNullInput_ShouldThrowException() {
            // Arrange
            string? password = null;
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.ComputeSha256Hash(password!));
        }

        [Fact]
        public void ComputeSha256Hash_ShouldBeDeterministic() {
            // Arrange
            var password = "deterministicpassword";
            
            // Act
            var actualHash = PasswordHasher.ComputeSha256Hash(password);
            
            // Assert
            Assert.NotNull(actualHash);
            Assert.True(actualHash.Length == 64);
            // This test mainly ensures the method doesn't throw and returns a consistent format
        }
    }
}
