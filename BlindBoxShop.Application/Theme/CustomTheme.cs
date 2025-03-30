using MudBlazor;

namespace BlindBoxShop.Application.Theme
{
    public static class CustomTheme
    {
        // Main primary color - Teal
        private static readonly string _primaryColor = "#008b8b"; // Teal (Dark Cyan)
        private static readonly string _secondaryColor = "#20b2aa"; // Medium Turquoise
        private static readonly string _tertiaryColor = "#48d1cc"; // Medium Turquoise (Lighter)
        
        // Màu bổ sung
        private static readonly string _infoColor = "#0288D1";
        private static readonly string _successColor = "#10B981";
        private static readonly string _warningColor = "#FF9800";
        private static readonly string _errorColor = "#F44336";
        
        // Màu văn bản
        private static readonly string _textPrimaryColor = "#424242";
        private static readonly string _textSecondaryColor = "#757575";
        
        public static MudTheme Theme => new MudTheme();

        // All theme colors are defined in CSS variables in MainLayout.razor for easier manipulation
        // See: BlindBoxShop.Application/Components/Layout/MainLayout.razor
    }
} 