# OpenAPI Spec Sync

This repository now tracks the upstream [kintone REST API spec](https://github.com/kintone/rest-api-spec) so that we can spot API additions and regenerate helpers quickly.

## How to fetch the latest spec

```bash
# Fetch the newest version that exists in the upstream repo
$ dotnet run --project tools/SpecSync

# Or fetch a specific version by timestamp
$ dotnet run --project tools/SpecSync -- --version 20240101000000
```

The command does the following:

1. Queries GitHub for the available spec versions (the latest timestamp is used unless `--version` is provided).
2. Downloads both the root `openapi.yaml` and the fully bundled spec (`bundled/openapi.yaml`).
3. Stores the files under `openapi/kintone/<version>/` and writes `openapi/kintone/version.txt`.
4. Parses the bundled spec and generates `src/Generated/RestApiSpec.fs`, which contains:
   - `SpecVersion`: the snapshot timestamp
   - `AllOperations`: the list of `<method, path, operationId, summary>` tuples from the spec
5. Refreshes `openapi/operation-coverage.json`, keeping an entry per `operationId`. New operations are added with the `status` field set to `unknown` so you can classify them manually.

You can reference `Kintone.Client.Generated.RestApiSpec.AllOperations` inside tests or tooling to detect which endpoints still need handcrafted client implementations.

## Typical workflow

1. Run `dotnet run --project tools/SpecSync` after the upstream repository publishes a new timestamp.
2. Inspect `git diff` to see which endpoints were added/changed (both in the YAML snapshot and in `AllOperations`).
3. Update the handwritten clients/test coverage accordingly.
   - For every new operation, edit `openapi/operation-coverage.json` and set `status` to one of `implemented`, `not-implemented`, `planned`, or `deprecated`.
   - Running `dotnet test` executes `OperationCoverageTests`, which fails if any entry still says `unknown`, if an operation is missing from the coverage file, or if the file references an operation that no longer exists in the spec.
4. Commit both the regenerated files and your implementation changes.

Because the generated file is part of the main library project, CI can fail fast whenever we forget to regenerate it for a new spec.

## Operation coverage quick reference

- File: `openapi/operation-coverage.json`
- Fields: `operationId`, `method`, `path`, `status`, `notes`
- Allowed statuses:
  - `implemented`: there is client support in this repository.
  - `not-implemented`: intentionally unsupported at the moment.
  - `planned`: work in progress.
  - `deprecated`: legacy endpoints we keep for reference.
- Unknown statuses trigger the `OperationCoverageTests` unit test, so remember to classify new operations before pushing.

## 自動ウォッチ（Spec Watch ワークフロー）

- `.github/workflows/spec-watch.yaml` が毎日 02:00 UTC に実行され、最新の upstream spec バージョンをポーリングします。
- `openapi/kintone/version.txt` に記録されたバージョンより新しいタイムスタンプを検出すると、自動的に Issue を作成して更新作業を促します（同じタイトルの既存 Issue があれば再利用）。
- 必要に応じて `workflow_dispatch` から手動実行もできます。PR を開く前に downstream の spec を追随させたい場合は、手動でワークフローを走らせて Issue 化 → 対応という流れにすると便利です。
