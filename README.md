# Akatsuki.DifficultyCalculator

A simple ASP.NET app to calculate difficulty attributes for osu!, using lazer.

## Setup

To build the container:

```bash
docker build . -t difficulty-calculator:latest
```

To run the API on port 5432:

```bash
docker run -p 5432:80 difficulty-calculator:latest
```

## Usage

The API currently has a single endpoint: `/attributes`

It takes `BeatmapId`, `RulesetId` and `Mods` as query arguments, and returns lazer's `DifficultyAttributes` model for the respective rulesets.