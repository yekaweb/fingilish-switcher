import os
from PIL import Image, ImageDraw, ImageFont, ImageFilter

def create_industrial_icon():
    # Render at high resolution (1024x1024) with 4x supersampling, then downscale
    size = 1024
    img = Image.new("RGBA", (size, size), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # 1. Subtle drop shadow
    shadow_margin = 48
    shadow_rect = [shadow_margin, shadow_margin + 20, size - shadow_margin, size - shadow_margin + 20]
    draw.rounded_rectangle(shadow_rect, radius=210, fill=(0, 0, 0, 100))
    img = img.filter(ImageFilter.GaussianBlur(radius=24))
    draw = ImageDraw.Draw(img)

    # 2. Main squircle body (Industrial dark graphite)
    body_margin = 56
    body_rect = [body_margin, body_margin, size - body_margin, size - body_margin]
    
    # Base fill
    draw.rounded_rectangle(body_rect, radius=195, fill=(15, 23, 42, 255)) # #0F172A

    # Subtle top-down gradient simulation
    for i in range(120):
        alpha = int(255 * (1 - i / 120))
        inset = body_margin + i
        if inset < (size // 2):
            draw.rounded_rectangle(
                [body_margin + 2, body_margin + 2 + i, size - body_margin - 2, body_margin + 2 + i + 2],
                radius=max(10, 195 - i),
                fill=(30, 41, 59, int(35 * (1 - i / 120)))
            )

    # 3. Precision technical border (1px equivalent)
    border_pen = (51, 65, 85, 255) # #334155
    draw.rounded_rectangle(body_rect, radius=195, outline=border_pen, width=12)

    # Inner highlight line
    inner_rect = [body_margin + 12, body_margin + 12, size - body_margin - 12, size - body_margin - 12]
    draw.rounded_rectangle(inner_rect, radius=185, outline=(71, 85, 105, 120), width=4)

    # 4. Central Geometric Motif: The Industrial "F" + Bidirectional Arrows
    # Center is at (512, 512)
    # Vertical spine of the F
    spine_x1 = 310
    spine_x2 = 415
    spine_y1 = 280
    spine_y2 = 744
    draw.rounded_rectangle([spine_x1, spine_y1, spine_x2, spine_y2], radius=18, fill=(248, 250, 252, 255))

    # Top horizontal arrow (Right-pointing) -> Electric Cyan #38BDF8
    top_bar_y1 = 280
    top_bar_y2 = 385
    top_bar_x1 = 310
    top_bar_x2 = 620
    draw.rounded_rectangle([top_bar_x1, top_bar_y1, top_bar_x2, top_bar_y2], radius=18, fill=(56, 189, 248, 255))
    
    # Top arrow head
    arrow1 = [
        (top_bar_x2 - 10, top_bar_y1 - 42),
        (top_bar_x2 + 95, top_bar_y1 + 52),
        (top_bar_x2 - 10, top_bar_y2 + 42)
    ]
    draw.polygon(arrow1, fill=(56, 189, 248, 255))

    # Center horizontal arrow (Left-pointing) -> Precision Signal Emerald #10B981 or Silver Teal #34D399
    mid_bar_y1 = 465
    mid_bar_y2 = 565
    mid_bar_x1 = 310
    mid_bar_x2 = 560
    draw.rounded_rectangle([mid_bar_x1, mid_bar_y1, mid_bar_x2, mid_bar_y2], radius=16, fill=(52, 211, 153, 255))

    # Opposing left arrow head under the stem
    arrow2 = [
        (spine_x1 + 10, mid_bar_y1 - 36),
        (spine_x1 - 85, mid_bar_y1 + 50),
        (spine_x1 + 10, mid_bar_y2 + 36)
    ]
    draw.polygon(arrow2, fill=(52, 211, 153, 255))

    # Precision mechanical tactile dot (LED status badge)
    led_x = 640
    led_y = 660
    led_r = 38
    # Outer glow
    draw.ellipse([led_x - led_r - 10, led_y - led_r - 10, led_x + led_r + 10, led_y + led_r + 10], fill=(56, 189, 248, 60))
    # Core
    draw.ellipse([led_x - led_r, led_y - led_r, led_x + led_r, led_y + led_r], fill=(56, 189, 248, 255))
    draw.ellipse([led_x - 14, led_y - 14, led_x + 14, led_y + 14], fill=(255, 255, 255, 220))

    # Save high-res PNG preview
    output_png = "d:/Fingilish/src/FingilishSwitcher/app_icon_1024.png"
    img.save(output_png, format="PNG")

    # Generate multi-layer Windows ICO
    ico_sizes = [(256, 256), (128, 128), (64, 64), (48, 48), (32, 32), (16, 16)]
    ico_images = []
    for s in ico_sizes:
        ico_images.append(img.resize(s, Image.Resampling.LANCZOS))

    output_ico = "d:/Fingilish/src/FingilishSwitcher/app.ico"
    ico_images[0].save(output_ico, format="ICO", sizes=ico_sizes, append_images=ico_images[1:])
    print(f"Generated multi-resolution ICO at: {output_ico}")

if __name__ == "__main__":
    create_industrial_icon()
