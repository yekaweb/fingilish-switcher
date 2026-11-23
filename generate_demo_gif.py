import os
import shutil
import subprocess
from PIL import Image, ImageDraw, ImageFont

# Paths
BASE_DIR = r"d:\Fingilish"
FONT_DIR = os.path.join(BASE_DIR, "src", "FingilishSwitcher", "Resources", "Fonts")
ASSETS_DIR = os.path.join(BASE_DIR, "assets")
FRAMES_DIR = os.path.join(BASE_DIR, "temp_frames")

os.makedirs(ASSETS_DIR, exist_ok=True)
if os.path.exists(FRAMES_DIR):
    shutil.rmtree(FRAMES_DIR)
os.makedirs(FRAMES_DIR, exist_ok=True)

# Dimensions (920x420 for crisp GitHub README display)
W, H = 920, 420
FPS = 25

# Load fonts
VAZIR_REG_PATH = os.path.join(FONT_DIR, "Vazirmatn-Regular.ttf")
VAZIR_MED_PATH = os.path.join(FONT_DIR, "Vazirmatn-Medium.ttf")
VAZIR_BOLD_PATH = os.path.join(FONT_DIR, "Vazirmatn-Bold.ttf")
SEGOE_UI_PATH = r"C:\Windows\Fonts\segoeui.ttf"
SEGOE_BOLD_PATH = r"C:\Windows\Fonts\segoeuib.ttf"

font_en_prompt = ImageFont.truetype(SEGOE_UI_PATH, 23)
font_fa_prompt = ImageFont.truetype(VAZIR_MED_PATH, 24)
font_ui_sm = ImageFont.truetype(SEGOE_UI_PATH, 15)
font_ui_sm_bold = ImageFont.truetype(SEGOE_BOLD_PATH, 16)
font_fa_badge = ImageFont.truetype(VAZIR_BOLD_PATH, 14)
font_keycap = ImageFont.truetype(SEGOE_BOLD_PATH, 17)

# Texts
WRONG_TEXT = "lk ldohl fvl fi hkjhgdh hc ;[h fgdx f'dvl"
CORRECT_TEXT = "من میخام برم به انتالیا از کجا بلیط بگیرم"

# Colors
BG_CANVAS = (11, 12, 15)           # #0B0C0F outer background
BOX_BG = (22, 23, 28)              # #16171C prompt box fill
BOX_BORDER = (44, 46, 56)          # #2C2E38 normal border
TEXT_WHITE = (241, 245, 249)       # #F1F5F9 main text
TEXT_MUTED = (148, 163, 184)       # #94A3B8 toolbar muted text
TEXT_DIM = (100, 116, 139)         # #64748B dim gray
SELECT_BG = (37, 99, 235)          # #2563EB text selection highlight
SEND_BLUE = (37, 99, 235)          # #2563EB circular send button
CYAN_ACCENT = (56, 189, 248)       # #38BDF8 industrial cyan
EMERALD = (16, 185, 129)           # #10B981 success green
AMBER = (245, 158, 11)             # #F59E0B mistyped warning

def draw_chevron_down(draw, cx, cy, color, size=4):
    """Draws a crisp 'v' chevron icon."""
    draw.line([(cx - size, cy - 2), (cx, cy + size - 1), (cx + size, cy - 2)], fill=color, width=2)

def draw_plus_icon(draw, cx, cy, color, size=7):
    """Draws a crisp '+' icon."""
    draw.line([(cx - size, cy), (cx + size, cy)], fill=color, width=2)
    draw.line([(cx, cy - size), (cx, cy + size)], fill=color, width=2)

def draw_shield_icon(draw, cx, cy, color):
    """Draws a minimal shield icon next to Turbo."""
    pts = [
        (cx, cy - 7),
        (cx + 6, cy - 5),
        (cx + 6, cy + 1),
        (cx, cy + 7),
        (cx - 6, cy + 1),
        (cx - 6, cy - 5),
    ]
    draw.polygon(pts, outline=color, width=2)

def draw_mic_icon(draw, cx, cy, color):
    """Draws a minimal microphone icon."""
    draw.rounded_rectangle([cx - 3, cy - 7, cx + 3, cy + 1], radius=3, outline=color, width=2)
    draw.arc([cx - 6, cy - 4, cx + 6, cy + 5], start=0, end=180, fill=color, width=2)
    draw.line([(cx, cy + 5), (cx, cy + 8)], fill=color, width=2)

def draw_send_arrow(draw, cx, cy, color):
    """Draws a crisp right arrow '->' inside the send button."""
    draw.line([(cx - 6, cy), (cx + 6, cy)], fill=color, width=2)
    draw.line([(cx + 1, cy - 5), (cx + 6, cy), (cx + 1, cy + 5)], fill=color, width=2)

def draw_swap_arrows(draw, cx, cy, color, scale=1.0):
    """Draws crisp bidirectional swap arrows (⇄) using pure vector lines."""
    dx = int(8 * scale)
    dy = int(4 * scale)
    ah = int(3 * scale)
    # Top arrow pointing right
    draw.line([(cx - dx, cy - dy), (cx + dx, cy - dy)], fill=color, width=2)
    draw.line([(cx + dx - ah, cy - dy - ah), (cx + dx, cy - dy), (cx + dx - ah, cy - dy + ah)], fill=color, width=2)
    # Bottom arrow pointing left
    draw.line([(cx - dx, cy + dy), (cx + dx, cy + dy)], fill=color, width=2)
    draw.line([(cx - dx + ah, cy + dy - ah), (cx - dx, cy + dy), (cx - dx + ah, cy + dy + ah)], fill=color, width=2)

def draw_checkmark(draw, cx, cy, color):
    """Draws a crisp vector checkmark."""
    draw.line([(cx - 6, cy), (cx - 2, cy + 4), (cx + 6, cy - 4)], fill=color, width=2)

def draw_mouse_cursor(draw, x, y, clicking=False):
    """Draws a crisp vector white mouse pointer with dark border at (x, y)."""
    pts = [
        (x, y),
        (x, y + 22),
        (x + 6, y + 17),
        (x + 11, y + 26),
        (x + 14, y + 24),
        (x + 9, y + 15),
        (x + 16, y + 15),
    ]
    shadow_pts = [(px + 1, py + 1) for (px, py) in pts]
    draw.polygon(shadow_pts, fill=(0, 0, 0))
    fill_col = CYAN_ACCENT if clicking else (255, 255, 255)
    draw.polygon(pts, fill=fill_col, outline=(15, 23, 42))

def draw_floating_bubble(draw, x, y, highlight=False):
    """Draws the Fingilish Switcher floating [ ⇄ ] button near the selection."""
    bw, bh = 44, 34
    bx, by = x, y
    bg = (30, 41, 59) if highlight else (15, 23, 42)
    border = CYAN_ACCENT if highlight else (100, 116, 139)
    draw.rounded_rectangle([bx, by, bx + bw, by + bh], radius=8, fill=bg, outline=border, width=2)
    draw_swap_arrows(draw, bx + bw // 2, by + bh // 2, CYAN_ACCENT if highlight else TEXT_WHITE, scale=1.0)

def draw_keycaps_hud(draw, pressed=False):
    """Draws the Ctrl + Shift + F shortcut overlay at the bottom center."""
    hud_w, hud_h = 370, 62
    hud_x = (W - hud_w) // 2
    hud_y = H - 84

    draw.rounded_rectangle(
        [hud_x, hud_y, hud_x + hud_w, hud_y + hud_h],
        radius=14,
        fill=(15, 23, 42),
        outline=CYAN_ACCENT if pressed else (51, 65, 85),
        width=2 if pressed else 1
    )

    keys = [("Ctrl", 74), ("Shift", 80), ("F", 54)]
    cur_x = hud_x + 36
    ky = hud_y + (13 if pressed else 10)
    kh = 40

    for i, (label, kw) in enumerate(keys):
        if not pressed:
            draw.rounded_rectangle(
                [cur_x, ky + 3, cur_x + kw, ky + kh + 3],
                radius=8,
                fill=(2, 6, 23)
            )
        k_bg = (14, 116, 144) if pressed else (30, 41, 59)
        k_border = CYAN_ACCENT if pressed else (100, 116, 139)
        draw.rounded_rectangle(
            [cur_x, ky, cur_x + kw, ky + kh],
            radius=8,
            fill=k_bg,
            outline=k_border,
            width=2 if pressed else 1
        )
        draw.text(
            (cur_x + kw // 2, ky + kh // 2),
            label,
            font=font_keycap,
            fill=(255, 255, 255),
            anchor="mm"
        )
        cur_x += kw
        if i < len(keys) - 1:
            draw.text((cur_x + 15, ky + kh // 2), "+", font=font_keycap, fill=TEXT_MUTED, anchor="mm")
            cur_x += 30

def render_frame(frame_idx):
    img = Image.new("RGB", (W, H), BG_CANVAS)
    draw = ImageDraw.Draw(img)

    # Timeline breakdown (225 frames = 9.0 seconds @ 25fps)
    is_converted = frame_idx >= 169
    is_pressing_hotkey = 152 <= frame_idx < 169
    show_hotkey_hud = 141 <= frame_idx < 182
    flash_convert = 169 <= frame_idx <= 175

    # 1. Top Header Bar
    # Mini brand icon box
    draw.rounded_rectangle([44, 20, 72, 48], radius=7, fill=(15, 23, 42), outline=CYAN_ACCENT, width=1)
    draw_swap_arrows(draw, 58, 34, CYAN_ACCENT, scale=0.75)

    draw.text((82, 34), "Fingilish Switcher", font=font_ui_sm_bold, fill=CYAN_ACCENT, anchor="lm")
    draw.text((232, 34), "تبدیل آنی متون اشتباه تایپ‌شده در هر محیط ویندوز", font=font_fa_badge, fill=TEXT_MUTED, anchor="lm")

    # Right status badge
    if not is_converted:
        badge_bg = (45, 30, 15)
        badge_border = AMBER
        badge_text = "کیبورد روی انگلیسی جا مانده (تایپ اشتباه)"
        badge_col = (252, 211, 77)
        draw.rounded_rectangle([W - 360, 18, W - 44, 50], radius=16, fill=badge_bg, outline=badge_border, width=1)
        draw.text((W - 202, 34), badge_text, font=font_fa_badge, fill=badge_col, anchor="mm")
    else:
        badge_bg = (6, 46, 34)
        badge_border = EMERALD
        badge_text = "اصلاح و جایگزینی هوشمند انجام شد"
        badge_col = (110, 231, 183)
        draw.rounded_rectangle([W - 360, 18, W - 44, 50], radius=16, fill=badge_bg, outline=badge_border, width=1)
        draw_checkmark(draw, W - 335, 34, EMERALD)
        draw.text((W - 190, 34), badge_text, font=font_fa_badge, fill=badge_col, anchor="mm")

    # 2. Main Prompt Box Container (styled after the user's screenshot)
    box_x1, box_y1 = 44, 68
    box_x2, box_y2 = W - 44, 315

    border_col = BOX_BORDER
    border_w = 2
    if flash_convert:
        border_col = EMERALD
        border_w = 3
    elif is_pressing_hotkey:
        border_col = CYAN_ACCENT
        border_w = 2

    draw.rounded_rectangle(
        [box_x1, box_y1, box_x2, box_y2],
        radius=20,
        fill=BOX_BG,
        outline=border_col,
        width=border_w
    )

    # 3. Bottom Toolbar inside Prompt Box (matching screenshot with pure vector icons)
    tb_y = box_y2 - 36
    # '+' button
    draw_plus_icon(draw, box_x1 + 28, tb_y, TEXT_MUTED, size=6)
    # Model selector label
    draw.text((box_x1 + 52, tb_y), "Gemini 3.8 Flash", font=font_ui_sm, fill=TEXT_MUTED, anchor="lm")
    draw.text((box_x1 + 172, tb_y), "High", font=font_ui_sm, fill=TEXT_DIM, anchor="lm")
    draw_chevron_down(draw, box_x1 + 216, tb_y + 1, TEXT_DIM, size=4)

    # Right toolbar items
    # Blue Send Circle button
    send_cx, send_cy, send_r = box_x2 - 34, tb_y, 18
    send_col = EMERALD if is_converted else SEND_BLUE
    draw.ellipse([send_cx - send_r, send_cy - send_r, send_cx + send_r, send_cy + send_r], fill=send_col)
    draw_send_arrow(draw, send_cx, send_cy, (255, 255, 255))

    # Mic vector icon
    draw_mic_icon(draw, box_x2 - 74, tb_y, TEXT_MUTED)

    # Shield + Turbo + Chevron vector icons
    draw_shield_icon(draw, box_x2 - 168, tb_y, TEXT_MUTED)
    draw.text((box_x2 - 155, tb_y), "Turbo", font=font_ui_sm, fill=TEXT_MUTED, anchor="lm")
    draw_chevron_down(draw, box_x2 - 104, tb_y + 1, TEXT_DIM, size=4)

    # 4. Text & Blinking Cursor inside Prompt Box
    text_y = box_y1 + 64
    left_margin = box_x1 + 42

    # Blinking cursor state
    cursor_visible = ((frame_idx // 10) % 2 == 0)

    if not is_converted:
        if frame_idx < 15:
            typed_str = ""
        elif frame_idx < 85:
            chars_count = int(((frame_idx - 15) / 70.0) * len(WRONG_TEXT)) + 1
            chars_count = min(chars_count, len(WRONG_TEXT))
            typed_str = WRONG_TEXT[:chars_count]
            cursor_visible = True
        else:
            typed_str = WRONG_TEXT

        bbox = draw.textbbox((0, 0), typed_str, font=font_en_prompt) if typed_str else (0, 0, 0, 0)
        text_w = bbox[2] - bbox[0]

        # Selection state (frames 112..168)
        selection_ratio = 0.0
        if 112 <= frame_idx < 140:
            selection_ratio = (frame_idx - 112) / 27.0
            selection_ratio = max(0.0, min(1.0, selection_ratio))
        elif frame_idx >= 140:
            selection_ratio = 1.0

        if selection_ratio > 0.0:
            sel_x2 = left_margin + text_w + 4
            sel_x1 = int(sel_x2 - (text_w + 8) * selection_ratio)
            draw.rounded_rectangle(
                [sel_x1, text_y - 18, sel_x2, text_y + 20],
                radius=5,
                fill=SELECT_BG
            )
            cursor_visible = False

        # Draw mistyped English string
        draw.text((left_margin, text_y), typed_str, font=font_en_prompt, fill=TEXT_WHITE, anchor="lm")

        # Draw blinking cursor at the end of typed_str
        if cursor_visible:
            cx = left_margin + text_w + 4
            draw.line([(cx, text_y - 14), (cx, text_y + 16)], fill=CYAN_ACCENT, width=2)

        # Mouse movement & selection drag animation
        if 100 <= frame_idx < 169:
            end_x = left_margin + text_w + 2
            start_x = left_margin
            if frame_idx < 112:
                t = (frame_idx - 100) / 12.0
                mx = int(end_x + 110 * (1.0 - t))
                my = int(text_y + 8 + 65 * (1.0 - t))
                draw_mouse_cursor(draw, mx, my, clicking=False)
            elif frame_idx < 140:
                mx = int(end_x - (end_x - start_x) * selection_ratio)
                my = text_y + 6
                draw_mouse_cursor(draw, mx, my, clicking=True)
            else:
                mx = start_x
                my = text_y + 6
                draw_floating_bubble(draw, mx - 2, text_y - 55, highlight=is_pressing_hotkey)
                draw_mouse_cursor(draw, mx, my, clicking=False)

    else:
        # CONVERTED STATE: Display Persian text in-place at left_margin where the selection was!
        fa_bbox = draw.textbbox((0, 0), CORRECT_TEXT, font=font_fa_prompt, direction="rtl")
        fa_w = fa_bbox[2] - fa_bbox[0]

        if flash_convert:
            draw.rounded_rectangle(
                [left_margin - 4, text_y - 18, left_margin + fa_w + 8, text_y + 20],
                radius=6,
                fill=(6, 78, 59)
            )

        # Draw Persian sentence in-place at left_margin
        draw.text(
            (left_margin, text_y),
            CORRECT_TEXT,
            font=font_fa_prompt,
            fill=TEXT_WHITE,
            anchor="lm",
            direction="rtl"
        )

        # Blinking cursor at the end (left side, after 'بگیرم') of the replaced Persian text
        if cursor_visible:
            cx = left_margin - 6
            draw.line([(cx, text_y - 14), (cx, text_y + 16)], fill=EMERALD, width=2)

    # 5. Draw Hotkey HUD (Ctrl + Shift + F)
    if show_hotkey_hud:
        draw_keycaps_hud(draw, pressed=is_pressing_hotkey)

    return img

def main():
    total_frames = 225
    print(f"Rendering {total_frames} frames...")
    for idx in range(total_frames):
        frame = render_frame(idx)
        frame_path = os.path.join(FRAMES_DIR, f"frame_{idx:04d}.png")
        frame.save(frame_path)

    # Save key preview frames for visual inspection
    shutil.copy(os.path.join(FRAMES_DIR, "frame_0060.png"), os.path.join(ASSETS_DIR, "preview_typing.png"))
    shutil.copy(os.path.join(FRAMES_DIR, "frame_0155.png"), os.path.join(ASSETS_DIR, "preview_selecting_hotkey.png"))
    shutil.copy(os.path.join(FRAMES_DIR, "frame_0185.png"), os.path.join(ASSETS_DIR, "preview_converted.png"))

    gif_path = os.path.join(ASSETS_DIR, "demo.gif")
    palette_path = os.path.join(FRAMES_DIR, "palette.png")

    cmd_palette = [
        "ffmpeg", "-y",
        "-framerate", str(FPS),
        "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
        "-vf", "palettegen=stats_mode=diff",
        "-update", "1",
        palette_path
    ]
    subprocess.run(cmd_palette, check=True)

    cmd_gif = [
        "ffmpeg", "-y",
        "-framerate", str(FPS),
        "-i", os.path.join(FRAMES_DIR, "frame_%04d.png"),
        "-i", palette_path,
        "-lavfi", "paletteuse=dither=bayer:bayer_scale=5:diff_mode=rectangle",
        gif_path
    ]
    subprocess.run(cmd_gif, check=True)

    shutil.rmtree(FRAMES_DIR, ignore_errors=True)
    print(f"Done! Saved GIF to {gif_path} ({os.path.getsize(gif_path) / 1024:.1f} KB)")

if __name__ == "__main__":
    main()
