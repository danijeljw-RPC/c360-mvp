#!/usr/bin/env zsh
set -euo pipefail

# Generates a self-contained mock-data/index.html file.
# No absolute local file paths are written into the generated HTML.

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"

if [[ -d "src/Cinturon360.Mock.Web/wwwroot/mock-data" ]]; then
  DATA_ROOT="src/Cinturon360.Mock.Web/wwwroot/mock-data"
else
  DATA_ROOT="$SCRIPT_DIR"
fi

OUT_FILE="${DATA_ROOT}/index.html"
URL_PREFIX="/mock-data"

if [[ ! -d "$DATA_ROOT" ]]; then
  echo "ERROR: Directory not found: $DATA_ROOT" >&2
  exit 1
fi

if ! command -v python3 >/dev/null 2>&1; then
  echo "ERROR: python3 is required" >&2
  exit 1
fi

python3 - "$DATA_ROOT" "$OUT_FILE" "$URL_PREFIX" <<'PY'
import csv
import html
import json
import sys
from pathlib import Path
from urllib.parse import quote

data_root = Path(sys.argv[1]).resolve()
out_file = Path(sys.argv[2]).resolve()
url_prefix = sys.argv[3].rstrip("/")

skip_names = {
    "index.html",
    "generate-mock-data-index.zsh",
}

files = []

for path in sorted(data_root.rglob("*")):
    if not path.is_file():
        continue

    rel = path.relative_to(data_root).as_posix()

    if path.name in skip_names:
        continue

    category = rel.split("/", 1)[0] if "/" in rel else "root"
    ext = path.suffix.lower().lstrip(".") or "file"
    mapped_url = f"{url_prefix}/" + "/".join(quote(part) for part in rel.split("/"))

    try:
        raw = path.read_text(encoding="utf-8")
    except UnicodeDecodeError:
        raw = path.read_text(encoding="utf-8", errors="replace")

    display_text = raw
    parsed_csv = None

    if ext == "json":
        try:
            display_text = json.dumps(json.loads(raw), indent=2, ensure_ascii=False)
        except Exception:
            pass

    if ext == "csv":
        try:
            rows = list(csv.reader(raw.splitlines()))
            if rows:
                parsed_csv = rows
        except Exception:
            pass

    files.append({
        "category": category,
        "relativePath": rel,
        "fileName": path.name,
        "extension": ext,
        "url": mapped_url,
        "content": display_text,
        "parsedCsv": parsed_csv,
    })

categories = sorted(set(f["category"] for f in files))

def e(value):
    return html.escape(str(value), quote=True)

def make_file_id(rel):
    return "file-" + "".join(ch if ch.isalnum() else "-" for ch in rel).strip("-").lower()

def csv_table(rows):
    if not rows:
        return ""

    header = rows[0]
    body = rows[1:]

    parts = [
        '<div class="csv-table-wrap">',
        '<table class="table csv-table">',
        "<thead><tr>",
    ]

    for cell in header:
        parts.append(f"<th>{e(cell)}</th>")

    parts.append("</tr></thead>")
    parts.append("<tbody>")

    for row in body:
        parts.append("<tr>")
        for index in range(len(header)):
            value = row[index] if index < len(row) else ""
            parts.append(f"<td>{e(value)}</td>")
        parts.append("</tr>")

    parts.append("</tbody>")
    parts.append("</table>")
    parts.append("</div>")

    return "\n".join(parts)

total_json = sum(1 for f in files if f["extension"] == "json")
total_csv = sum(1 for f in files if f["extension"] == "csv")

html_parts = []

html_parts.append(f"""<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Cinturon360 Mock Data Index</title>
  <style>
    :root {{
      --bg: #0f172a;
      --panel: #111827;
      --panel-2: #020617;
      --text: #e5e7eb;
      --muted: #94a3b8;
      --border: rgba(148, 163, 184, 0.24);
      --accent: #38bdf8;
      --accent-2: #f97316;
      --code-bg: #020617;
      --shadow: 0 18px 50px rgba(0,0,0,.32);
      --radius: 18px;
    }}

    * {{
      box-sizing: border-box;
    }}

    body {{
      margin: 0;
      min-height: 100vh;
      background:
        radial-gradient(circle at top left, rgba(56, 189, 248, 0.18), transparent 34rem),
        radial-gradient(circle at top right, rgba(249, 115, 22, 0.14), transparent 32rem),
        var(--bg);
      color: var(--text);
      font-family: system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
      line-height: 1.5;
    }}

    a {{
      color: var(--accent);
      text-decoration: none;
    }}

    a:hover {{
      text-decoration: underline;
    }}

    code,
    pre,
    .pill,
    .table td {{
      font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
    }}

    .container {{
      width: min(1480px, calc(100vw - 32px));
      margin: 0 auto;
      padding: 32px 0 60px;
    }}

    .hero {{
      display: grid;
      gap: 18px;
      grid-template-columns: 1fr auto;
      align-items: start;
      padding: 28px;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: linear-gradient(135deg, rgba(17, 24, 39, .95), rgba(2, 6, 23, .92));
      box-shadow: var(--shadow);
    }}

    .brand {{
      margin: 0;
      font-size: clamp(2rem, 4vw, 3.6rem);
      line-height: 1;
      letter-spacing: -0.05em;
      font-weight: 780;
    }}

    .brand .blue {{
      color: var(--accent);
    }}

    .brand .orange {{
      color: var(--accent-2);
    }}

    .subtitle {{
      margin: 10px 0 0;
      color: var(--muted);
      font-size: 1rem;
      max-width: 900px;
    }}

    .badge-row {{
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      margin-top: 18px;
    }}

    .badge {{
      display: inline-flex;
      align-items: center;
      padding: 7px 11px;
      border-radius: 999px;
      border: 1px solid var(--border);
      background: rgba(15, 23, 42, .72);
      font-size: .85rem;
    }}

    .badge strong {{
      color: white;
      margin-right: 6px;
    }}

    .actions,
    .file-actions {{
      display: flex;
      flex-wrap: wrap;
      gap: 10px;
      justify-content: flex-end;
    }}

    .btn {{
      display: inline-flex;
      align-items: center;
      justify-content: center;
      border: 1px solid var(--border);
      background: rgba(15, 23, 42, .85);
      color: var(--text);
      border-radius: 12px;
      padding: 10px 14px;
      font-size: .92rem;
      cursor: pointer;
      min-height: 42px;
    }}

    .btn:hover {{
      border-color: rgba(56, 189, 248, .6);
      text-decoration: none;
    }}

    .btn-primary {{
      background: linear-gradient(135deg, rgba(56, 189, 248, .22), rgba(249, 115, 22, .16));
      border-color: rgba(56, 189, 248, .45);
    }}

    .layout {{
      display: grid;
      grid-template-columns: 320px 1fr;
      gap: 22px;
      margin-top: 22px;
      align-items: start;
    }}

    .sidebar {{
      position: sticky;
      top: 18px;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: rgba(17, 24, 39, .9);
      box-shadow: var(--shadow);
      overflow: hidden;
    }}

    .sidebar-header {{
      padding: 18px;
      border-bottom: 1px solid var(--border);
      background: rgba(2, 6, 23, .45);
    }}

    .sidebar-header h2 {{
      margin: 0;
      font-size: 1rem;
    }}

    .search {{
      width: 100%;
      margin-top: 12px;
      padding: 11px 12px;
      border: 1px solid var(--border);
      border-radius: 12px;
      background: var(--panel-2);
      color: var(--text);
      outline: none;
    }}

    .tabs {{
      display: flex;
      flex-direction: column;
      gap: 4px;
      padding: 10px;
    }}

    .tab {{
      display: flex;
      justify-content: space-between;
      width: 100%;
      padding: 10px 12px;
      border: 1px solid transparent;
      border-radius: 12px;
      background: transparent;
      color: var(--text);
      text-align: left;
      cursor: pointer;
      font-size: .95rem;
    }}

    .tab:hover,
    .tab.active {{
      background: rgba(56, 189, 248, .1);
      border-color: rgba(56, 189, 248, .28);
    }}

    .tab-count {{
      color: var(--muted);
      font-variant-numeric: tabular-nums;
    }}

    .content {{
      display: grid;
      gap: 18px;
    }}

    .category-panel {{
      display: none;
      gap: 18px;
    }}

    .category-panel.active {{
      display: grid;
    }}

    .category-title {{
      margin: 0;
      padding: 18px 20px;
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: rgba(17, 24, 39, .88);
      box-shadow: var(--shadow);
      font-size: 1.35rem;
      text-transform: uppercase;
      letter-spacing: .08em;
    }}

    .file-card {{
      border: 1px solid var(--border);
      border-radius: var(--radius);
      background: rgba(17, 24, 39, .92);
      box-shadow: var(--shadow);
      overflow: hidden;
    }}

    .file-card.hidden {{
      display: none;
    }}

    .file-header {{
      display: grid;
      grid-template-columns: 1fr auto;
      gap: 12px;
      padding: 18px 20px;
      border-bottom: 1px solid var(--border);
      background: rgba(2, 6, 23, .38);
    }}

    .file-title {{
      margin: 0;
      font-size: 1.05rem;
      line-height: 1.25;
      word-break: break-word;
    }}

    .file-meta {{
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-top: 10px;
    }}

    .pill {{
      display: inline-flex;
      align-items: center;
      padding: 5px 9px;
      border: 1px solid var(--border);
      border-radius: 999px;
      color: var(--muted);
      font-size: .78rem;
      background: rgba(15, 23, 42, .7);
      word-break: break-all;
    }}

    details {{
      border-top: 1px solid var(--border);
    }}

    summary {{
      cursor: pointer;
      padding: 14px 20px;
      font-weight: 650;
      background: rgba(15, 23, 42, .56);
    }}

    summary:hover {{
      background: rgba(56, 189, 248, .08);
    }}

    pre {{
      margin: 0;
      padding: 18px 20px;
      max-height: 520px;
      overflow: auto;
      background: var(--code-bg);
      color: #dbeafe;
      font-size: .87rem;
      line-height: 1.55;
      tab-size: 2;
    }}

    .csv-table-wrap {{
      max-height: 520px;
      overflow: auto;
      background: var(--code-bg);
    }}

    table {{
      width: 100%;
      border-collapse: collapse;
    }}

    .table th,
    .table td {{
      padding: 9px 11px;
      border-bottom: 1px solid rgba(148, 163, 184, .16);
      text-align: left;
      vertical-align: top;
      font-size: .88rem;
    }}

    .table th {{
      position: sticky;
      top: 0;
      z-index: 1;
      background: #0b1220;
      color: #f8fafc;
      white-space: nowrap;
    }}

    .table td {{
      color: #dbeafe;
    }}

    .table tbody tr:nth-child(even) {{
      background: rgba(148, 163, 184, .055);
    }}

    .empty {{
      padding: 28px;
      border: 1px dashed var(--border);
      border-radius: var(--radius);
      color: var(--muted);
      text-align: center;
      background: rgba(17, 24, 39, .55);
    }}

    .footer {{
      margin-top: 22px;
      color: var(--muted);
      font-size: .85rem;
      text-align: center;
    }}

    @media (max-width: 900px) {{
      .hero,
      .layout,
      .file-header {{
        grid-template-columns: 1fr;
      }}

      .actions,
      .file-actions {{
        justify-content: flex-start;
      }}

      .sidebar {{
        position: static;
      }}

      .tabs {{
        flex-direction: row;
        overflow-x: auto;
      }}

      .tab {{
        min-width: 180px;
      }}
    }}
  </style>
</head>
<body>
  <div class="container">
    <section class="hero">
      <div>
        <h1 class="brand"><span class="blue">Cinturon</span><span class="orange">360</span> Mock Data</h1>
        <p class="subtitle">
          Self-contained demo index for mock data files under <code>{e(url_prefix)}</code>.
          File previews are embedded directly into this HTML file.
        </p>

        <div class="badge-row">
          <span class="badge"><strong>{len(files)}</strong> files</span>
          <span class="badge"><strong>{len(categories)}</strong> categories</span>
          <span class="badge"><strong>{total_json}</strong> JSON</span>
          <span class="badge"><strong>{total_csv}</strong> CSV</span>
        </div>
      </div>

      <div class="actions">
        <button class="btn btn-primary" type="button" onclick="expandAll()">Expand all</button>
        <button class="btn" type="button" onclick="collapseAll()">Collapse all</button>
      </div>
    </section>

    <section class="layout">
      <aside class="sidebar">
        <div class="sidebar-header">
          <h2>Categories</h2>
          <input id="searchBox" class="search" type="search" placeholder="Filter files..." autocomplete="off">
        </div>

        <nav class="tabs" aria-label="Mock data category tabs">
""")

for index, category in enumerate(categories):
    count = sum(1 for f in files if f["category"] == category)
    active = " active" if index == 0 else ""
    html_parts.append(f"""          <button class="tab{active}" type="button" data-tab="{e(category)}">
            <span>{e(category)}</span>
            <span class="tab-count">{count}</span>
          </button>
""")

html_parts.append("""        </nav>
      </aside>

      <main class="content">
""")

for index, category in enumerate(categories):
    active = " active" if index == 0 else ""
    html_parts.append(f"""        <section class="category-panel{active}" data-panel="{e(category)}">
          <h2 class="category-title">{e(category)}</h2>
""")

    for f in [x for x in files if x["category"] == category]:
        file_id = make_file_id(f["relativePath"])
        rel = e(f["relativePath"])
        url = e(f["url"])
        content = e(f["content"])
        ext = e(f["extension"].upper())

        html_parts.append(f"""          <article class="file-card" data-file-card data-search="{e((f['relativePath'] + ' ' + f['url']).lower())}">
            <header class="file-header">
              <div>
                <h3 class="file-title" id="{e(file_id)}">{rel}</h3>
                <div class="file-meta">
                  <span class="pill">{ext}</span>
                  <span class="pill">path: {rel}</span>
                  <span class="pill">url: {url}</span>
                </div>
              </div>

              <div class="file-actions">
                <a class="btn btn-primary" href="{url}" download>Download</a>
                <a class="btn" href="{url}" target="_blank" rel="noopener">Open</a>
                <button class="btn" type="button" onclick="copyText('{e(file_id)}-content')">Copy</button>
              </div>
            </header>
""")

        if f["parsedCsv"]:
            html_parts.append(f"""            <details open>
              <summary>CSV table view</summary>
              {csv_table(f["parsedCsv"])}
            </details>
""")

        html_parts.append(f"""            <details open>
              <summary>Raw embedded content</summary>
              <pre id="{e(file_id)}-content"><code>{content}</code></pre>
            </details>
          </article>
""")

    html_parts.append("""        </section>
""")

html_parts.append("""        <div id="emptyState" class="empty" style="display:none;">
          No mock data files match the current filter.
        </div>
      </main>
    </section>

    <div class="footer">
      Generated index.html is self-contained. It does not include absolute local source paths.
    </div>
  </div>

  <script type="application/json" id="mock-data-index">
""")

safe_index = [
    {
        "category": f["category"],
        "relativePath": f["relativePath"],
        "fileName": f["fileName"],
        "extension": f["extension"],
        "url": f["url"],
        "content": f["content"],
    }
    for f in files
]

html_parts.append(e(json.dumps(safe_index, indent=2, ensure_ascii=False)))

html_parts.append("""
  </script>

  <script>
    const tabs = Array.from(document.querySelectorAll("[data-tab]"));
    const panels = Array.from(document.querySelectorAll("[data-panel]"));
    const searchBox = document.getElementById("searchBox");
    const emptyState = document.getElementById("emptyState");

    function selectTab(category) {
      tabs.forEach(tab => tab.classList.toggle("active", tab.dataset.tab === category));
      panels.forEach(panel => panel.classList.toggle("active", panel.dataset.panel === category));
      applyFilter();
    }

    function applyFilter() {
      const query = searchBox.value.trim().toLowerCase();
      const activePanel = document.querySelector(".category-panel.active");
      const cards = Array.from(activePanel.querySelectorAll("[data-file-card]"));

      let visible = 0;

      cards.forEach(card => {
        const match = !query || card.dataset.search.includes(query);
        card.classList.toggle("hidden", !match);
        if (match) visible++;
      });

      emptyState.style.display = visible === 0 ? "block" : "none";
    }

    function expandAll() {
      document.querySelectorAll("details").forEach(detail => detail.open = true);
    }

    function collapseAll() {
      document.querySelectorAll("details").forEach(detail => detail.open = false);
    }

    async function copyText(id) {
      const el = document.getElementById(id);
      if (!el) return;

      const text = el.innerText;

      try {
        await navigator.clipboard.writeText(text);
      } catch {
        const textarea = document.createElement("textarea");
        textarea.value = text;
        document.body.appendChild(textarea);
        textarea.select();
        document.execCommand("copy");
        textarea.remove();
      }
    }

    tabs.forEach(tab => {
      tab.addEventListener("click", () => selectTab(tab.dataset.tab));
    });

    searchBox.addEventListener("input", applyFilter);
  </script>
</body>
</html>
""")

out_file.write_text("".join(html_parts), encoding="utf-8")

print(f"Generated: {out_file}")
print(f"Indexed files: {len(files)}")
for category in categories:
    count = sum(1 for f in files if f["category"] == category)
    print(f"  {category}: {count}")
PY

echo
echo "Checking generated HTML for private absolute paths..."

if grep -q "$HOME" "$OUT_FILE"; then
  echo "ERROR: Generated index contains HOME path: $HOME" >&2
  exit 1
fi

if grep -q "/Users/" "$OUT_FILE"; then
  echo "ERROR: Generated index contains /Users/ path" >&2
  exit 1
fi

echo "OK: no absolute local user paths found."