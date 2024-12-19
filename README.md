# Unity WebGL Templates

Unity package containing WebGL templates with automatic resizing capabilities.

## Usage

Install the package according to the [installation instructions](#installation). Once installed, the templates should automatically be added to your Assets folder, then you can select the desired WebGL Templates in the Player settings.

Upon updating the package, you might be asked to update the previously imported assets. This will override all files that are being reimported, but not delete any which were deleted from one version to the next.

## Installation

### Option 1: Package Manager (recommended)

Open the Package Manager window, click on `Add Package from Git URL ...`, then enter the following:

```
https://github.com/lajawi/unity-webgl-templates.git
```

### Option 2: Manually Editing `package.json`

Add the following line to your project's `Packages/manifest.json`:

```json
"com.github.lajawi.webgltemplates": "https://github.com/lajawi/unity-webgl-templates.git"
```
