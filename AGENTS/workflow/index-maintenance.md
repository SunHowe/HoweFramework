# 索引维护

`AGENTS/` 与 `Doc/` 每一层目录都必须有 `README.md`，作为该层索引。根 [`AGENTS.md`](../../AGENTS.md) 不是索引，只指向两套总索引。

## 索引格式

每个 `README.md` 按此结构，保持短：

```markdown
# 目录标题

一两句：本目录用途。必要时链到父索引。

## 本目录用途

## 子文件

| 文件 | 一句话 |
|------|--------|
| [`foo.md`](foo.md) | … |

## 子目录

| 目录 | 用途 |
|------|------|
| [`bar/`](bar/README.md) | … |
```

没有子文件或没有子目录时，对应节写「无」，不要删节。

一句话说明该让后人决定「要不要点开」。不要在索引里重复正文。

## 何时更新

| 变化 | 要改的索引 |
|------|------------|
| 新建 / 重命名 / 删除某个 md | 该文件所在目录的 `README.md` |
| 新建 / 重命名 / 删除子目录 | 父目录 `README.md`（子目录自己也要有 README） |
| 某篇「一句话」已不能概括内容 | 只改该层 README 的那一行 |
| `Doc/` 顶层分区变了 | [`Doc/README.md`](../../Doc/README.md) 的「先读哪篇」 |
| Agent 流程分区变了 | [`AGENTS/README.md`](../README.md)；开工/完成清单变了再改根 `AGENTS.md` |

## 向上冒泡

改完当前层 README 后，检查父目录 README 的链接是否仍有效，直到：

- 知识变更 → `Doc/README.md`
- 流程变更 → `AGENTS/README.md`

不要跳层只改总索引、不管中间 README，也不要只改文件不改索引。

## 链接

- 同目录用相对路径：`[`event.md`](event.md)`
- 子目录索引必须链到 `README.md`：`[`modules/`](modules/README.md)`
- 不要链到已删除路径；不要链到 `Client/Assets/**/Doc` 当知识正文。
