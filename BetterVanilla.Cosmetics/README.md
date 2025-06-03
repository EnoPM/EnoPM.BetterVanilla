# 🎩 Hat Cosmetic Resource Naming Guide

This guide explains how to name cosmetic hat resource files so they can be correctly processed by the `CreateHatCosmeticApisFromFilenames` method in `BetterVanilla`.

---

## 📁 File Naming Format

```
<key><option1>_<option2>_...<extension>
```


- `<key>`: Unique identifier for the hat (e.g., `party-hat`)
- `<optionX>`: Optional behavior or variant flags
- Extension (e.g., `.png`) is ignored during parsing

If the method is called with `fromDisk = true`, the filename is extracted from the full path.

---

## 🧩 Available Options

| Option      | Description                                     |
|-------------|-------------------------------------------------|
| `back`      | Back-facing version of the hat                  |
| `flip`      | Flipped animation frame                         |
| `back+flip` | Back-facing **and** flipped variant             |
| `climb`     | Frame used during climbing animations           |
| `bounce`    | Enables bouncing animation                      |
| `adaptive`  | Enables adaptive coloring (e.g. color matching) |
| `behind`    | Forces the hat to render behind the character   |

> These options must appear after the key, separated by underscores.

---

## 🎯 Examples

| Filename                        | Description                                  |
|---------------------------------|----------------------------------------------|
| `party-hat.png`                 | Base front-facing hat                        |
| `party-hat_back.png`            | Back variant                                 |
| `party-hat_flip.png`            | Flipped front-facing frame                   |
| `party-hat_back_flip.png`       | Back-facing and flipped variant              |
| `party-hat_climb.png`           | Climbing animation frame                     |
| `party-hat_bounce_adaptive.png` | Bouncing and adaptive front-facing hat       |
| `party-hat_behind.png`          | Base hat that is forced behind the character |

---

## 🛠 Parsing Behavior

- **Base image (front)**: must not contain any reserved options. It will be the main entry for the cosmetic.
- **Variants**: are matched by key and added to the base hat object automatically.
- `behind` will be automatically enabled if a back image is present, regardless of filename.

---

## ⚠️ Notes

- File names are split using underscores `_`.
- Hyphens (`-`) in the `<key>` are **converted to spaces** for the final display name.
- All variants must use the **exact same key** to be grouped correctly.

---

## ✅ Summary

Use consistent, descriptive names following the `<key>_<option1>_<option2>` pattern to ensure your cosmetic resources are detected and parsed properly by the system.

