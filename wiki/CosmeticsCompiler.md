> **CosmeticsCompiler** is a command-line tool designed to streamline the creation and packaging of custom cosmetics for
> Among Us. It enables mod developers to preprocess and bundle custom cosmetic components (like hats and visors) into
> optimized, compressed binary files that can be loaded at runtime by the **CosmeticsPlugin**.

# 🔍 What It Does

## **CosmeticsCompiler** performs two main tasks:

### 1. Spritesheet Generation

It takes individual image files representing various cosmetic components (e.g., walking animations, flip views, climb
views) and compiles them into a spritesheet JSON map. This map describes how the different frames and resources are
structured and can be reused by other tools or directly by the CosmeticsCompiler bundler.
***

### 2. Bundle Compilation

It collects one or more cosmetic definition files (spritesheet maps) and all their associated image files, then packages
them into a single compressed binary bundle. This bundle is optimized for runtime usage and can be loaded by the *
*CosmeticsPlugin** to display custom hats and visors in the game.

# 🛠️ Usage

## 1. `create-hat-spritesheet`

> This command compiles hat components into a PNG spritesheet and a JSON definition file that maps all animations and
> views.

```shell
BetterVanilla.CosmeticsCompiler.exe create-hat-spritesheet \
  --name "My custom hat" \
  --author "My author name" \
  --main-resource "path/to/main.png" \
  --flip-resource "path/to/flip.png" \
  --back-resource "path/to/back.png" \
  --back-flip-resource "path/to/back_flip.png" \
  --climb-resource "path/to/climb.png" \
  --front-animation-frames "path/to/frame1.png" "path/to/frame2.png" \
  --back-animation-frames "path/to/frame1.png" "path/to/frame2.png" \
  --bounce \
  --adaptive \
  --no-visors \
  --output "path/to/output/directory"
```

### Options:

| Required | Option                     | Type             | Description                                                                  |
|----------|----------------------------|------------------|------------------------------------------------------------------------------|
| Yes      | `--main-resource`          | Path to PNG file | Main front-facing sprite                                                     |
| Yes      | `--name` `-n`              | string           | Hat name                                                                     |
| Yes      | `--output` `-o`            | Directory path   | Directory in which will be output `[name].png` and `[name].spritesheet.json` |
| No       | `--author`                 | string           | Name of the author of the hat                                                |
| No       | `--adaptive`               | boolean          | Whether the hat matches the player's colors                                  |
| No       | `--bounce`                 | boolean          | Whether the hat should bounce                                                |
| No       | `--no-visors`              | boolean          | Whether the hat is incompatible with visors                                  |
| No       | `--flip-resource`          | Path to PNG file | Flipped front sprite                                                         |
| No       | `--back-resource`          | Path to PNG file | Rear view of the hat                                                         |
| No       | `--climb-resource`         | Path to PNG file | Hat sprite used while climbing                                               |
| No       | `--front-animation-frames` | Path to PNG file | List of main animation frame sprites                                         |
| No       | `--back-animation-frames`  | Path to PNG file | List of rear animation frame sprites                                         |

## 2. `create-visor-spritesheet`

> This command compiles visor components into a PNG spritesheet and a JSON definition file that maps all animations and
> views.

```shell
BetterVanilla.CosmeticsCompiler.exe create-visor-spritesheet \
  --name "My custom visor" \
  --author "My author name" \
  --main-resource "path/to/main.png" \
  --left-resource "path/to/left.png" \
  --climb-resource "path/to/climb.png" \
  --floor-resource "path/to/floor.png" \
  --adaptive \
  --behind-hat \
  --output "path/to/output/directory"
```

### Options:

| Required | Option                     | Type             | Description                                                                  |
|----------|----------------------------|------------------|------------------------------------------------------------------------------|
| Yes      | `--main-resource`          | Path to PNG file | Main front-facing sprite                                                     |
| Yes      | `--name` `-n`              | string           | Hat name                                                                     |
| Yes      | `--output` `-o`            | Directory path   | Directory in which will be output `[name].png` and `[name].spritesheet.json` |
| No       | `--author`                 | string           | Name of the author of the hat                                                |
| No       | `--adaptive`               | boolean          | Whether the hat matches the player's colors                                  |
| No       | `--bounce`                 | boolean          | Whether the hat should bounce                                                |
| No       | `--no-visors`              | boolean          | Whether the hat is incompatible with visors                                  |
| No       | `--flip-resource`          | Path to PNG file | Flipped front sprite                                                         |
| No       | `--back-resource`          | Path to PNG file | Rear view of the hat                                                         |
| No       | `--climb-resource`         | Path to PNG file | Hat sprite used while climbing                                               |
| No       | `--front-animation-frames` | Path to PNG file | List of main animation frame sprites                                         |
| No       | `--back-animation-frames`  | Path to PNG file | List of rear animation frame sprites                                         |

## 3. `bundle`

> Once you have created one or more JSON spritesheet maps for hats and visors, you can bundle them into a single binary
> file.

```shell
BetterVanilla.CosmeticsCompiler.exe bundle \
  --output "path/to/bundle.cosmetics" \
  --compression \
  --hats "path/to/hat1.json" "path/to/hat2.json" \
  --visors "path/to/visor1.json" "path/to/visor2.json"
```

### Options:

| Required | Option          | Type                          | Description                                |
|----------|-----------------|-------------------------------|--------------------------------------------|
| Yes      | `--output` `-o` | File path                     | Output bundle file path                    |
| No       | `--compression` | boolean                       | If set, compresses the bundle using Brotli |
| No       | `--hats`        | Path to JSON spritesheet file | Paths to hat JSON spritesheet              |
| No       | `--bounce`      | Path to JSON spritesheet file | Paths to visor JSON spritesheet            |