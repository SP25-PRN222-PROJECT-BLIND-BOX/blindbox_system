// Cart animation functions

// Function to animate cart item removal
function animateCartItemRemoval(itemId) {
    const cartRow = document.querySelector(`[data-item-id="${itemId}"]`);
    if (cartRow) {
        cartRow.style.transition = 'all 0.3s ease';
        cartRow.style.opacity = '0';
        cartRow.style.transform = 'translateX(30px)';
    }
}

// Update quantity display animation
function updateQuantityAnimation(element) {
    const quantityElement = document.querySelector(element);
    if (quantityElement) {
        quantityElement.classList.add('quantity-changed');
        setTimeout(() => {
            quantityElement.classList.remove('quantity-changed');
        }, 500);
    }
}

// Product image hover effects
function setupProductHoverEffects() {
    const productImages = document.querySelectorAll('.product-image-container');
    
    productImages.forEach(container => {
        container.addEventListener('mouseenter', () => {
            const img = container.querySelector('img');
            if (img) {
                img.style.transform = 'scale(1.1)';
            }
        });
        
        container.addEventListener('mouseleave', () => {
            const img = container.querySelector('img');
            if (img) {
                img.style.transform = 'scale(1)';
            }
        });
    });
}

// Simulate loading animation when moving to checkout
function simulateCheckoutLoading() {
    // Create and show loading overlay
    const overlay = document.createElement('div');
    overlay.className = 'checkout-loading-overlay';
    overlay.innerHTML = `
        <div class="loading-spinner"></div>
        <p>Đang chuẩn bị thanh toán...</p>
    `;
    document.body.appendChild(overlay);
    
    // Remove overlay after animation
    setTimeout(() => {
        overlay.style.opacity = '0';
        setTimeout(() => {
            document.body.removeChild(overlay);
        }, 300);
    }, 1000);
    
    return true;
}

// Init function to be called when the page is loaded
function initCartAnimations() {
    // Add required CSS if not already present
    if (!document.getElementById('cart-animations-style')) {
        const style = document.createElement('style');
        style.id = 'cart-animations-style';
        style.textContent = `
            .quantity-changed {
                background-color: rgba(0, 201, 209, 0.2) !important;
                transition: background-color 0.5s ease;
            }
            .mud-table-row {
                transition: all 0.3s ease;
            }
            
            .checkout-loading-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(255, 255, 255, 0.9);
                display: flex;
                flex-direction: column;
                justify-content: center;
                align-items: center;
                z-index: 9999;
                transition: opacity 0.3s ease;
            }
            
            .loading-spinner {
                border: 4px solid rgba(0, 201, 209, 0.3);
                border-radius: 50%;
                border-top: 4px solid rgba(0, 201, 209, 1);
                width: 40px;
                height: 40px;
                animation: spin 1s linear infinite;
                margin-bottom: 1rem;
            }
            
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        `;
        document.head.appendChild(style);
    }
    
    // Setup product hover effects after a slight delay
    setTimeout(setupProductHoverEffects, 500);
}

// Export functions to window object for Blazor to access
window.animateCartItemRemoval = animateCartItemRemoval;
window.updateQuantityAnimation = updateQuantityAnimation;
window.initCartAnimations = initCartAnimations;
window.simulateCheckoutLoading = simulateCheckoutLoading; 