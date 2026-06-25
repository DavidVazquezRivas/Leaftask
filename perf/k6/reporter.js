function ms(v) { return v === undefined ? '-' : v.toFixed(1) + ' ms'; }
function pct(v) { return v === undefined ? '-' : (v * 100).toFixed(2) + '%'; }
function num(v) { return v === undefined ? '-' : Math.round(v).toLocaleString(); }

export function generateHtml(data, profile) {
    const m = data.metrics;
    const dur    = m['http_req_duration']?.values ?? {};
    const failed = m['http_req_failed']?.values   ?? {};
    const reqs   = m['http_reqs']?.values         ?? {};
    const iters  = m['iterations']?.values        ?? {};
    const vusMax = m['vus_max']?.values           ?? {};

    const checks = data.root_group?.checks ?? [];

    const thresholdRows = Object.entries(m)
        .filter(([, metric]) => metric.thresholds)
        .map(([name, metric]) => {
            const entries = Object.entries(metric.thresholds);
            return entries.map(([cond, result]) => {
                const ok = result.ok;
                return `<tr>
                    <td>${name}</td>
                    <td><code>${cond}</code></td>
                    <td class="${ok ? 'ok' : 'fail'}">${ok ? '✓ PASS' : '✗ FAIL'}</td>
                </tr>`;
            }).join('');
        }).join('');

    const checkRows = checks.map(c => {
        const total = c.passes + c.fails;
        const rate  = total ? (c.passes / total * 100).toFixed(1) : '0.0';
        const ok    = c.fails === 0;
        return `<tr>
            <td>${c.name}</td>
            <td>${num(c.passes)}</td>
            <td>${num(c.fails)}</td>
            <td class="${ok ? 'ok' : 'fail'}">${rate}%</td>
        </tr>`;
    }).join('');

    const now       = new Date().toISOString().replace('T', ' ').slice(0, 19) + ' UTC';
    const p95ok     = (dur['p(95)'] ?? 9999) < 500;
    const errorOk   = (failed.rate ?? 0) < 0.01;

    return `<!DOCTYPE html>
<html lang="es">
<head>
<meta charset="UTF-8">
<title>k6 Report · ${profile} · ${now.slice(0, 10)}</title>
<style>
  *, *::before, *::after { box-sizing: border-box; }
  body {
    font-family: system-ui, -apple-system, sans-serif;
    max-width: 900px; margin: 0 auto; padding: 2rem 1.5rem;
    background: #f0f4f8; color: #1e293b;
  }
  h1 { font-size: 1.4rem; margin: 0 0 .25rem; }
  .meta { font-size: .82rem; color: #64748b; margin-bottom: 1.75rem; }
  .badge {
    display: inline-block; padding: .2rem .55rem; border-radius: 9999px;
    font-size: .75rem; font-weight: 600; text-transform: uppercase;
    background: #dbeafe; color: #1d4ed8; margin-left: .5rem;
  }

  /* ── cards ── */
  .cards {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
    gap: .75rem; margin-bottom: 1.75rem;
  }
  .card {
    background: #fff; border-radius: 10px;
    padding: .9rem 1rem; box-shadow: 0 1px 3px rgba(0,0,0,.08);
  }
  .card .lbl { font-size: .7rem; color: #94a3b8; text-transform: uppercase; letter-spacing: .04em; }
  .card .val { font-size: 1.55rem; font-weight: 700; line-height: 1.2; margin-top: .2rem; }
  .green { color: #16a34a; }
  .red   { color: #dc2626; }

  /* ── section ── */
  h2 { font-size: .95rem; color: #475569; text-transform: uppercase;
       letter-spacing: .05em; margin: 1.5rem 0 .5rem; }

  table {
    width: 100%; border-collapse: collapse;
    background: #fff; border-radius: 10px; overflow: hidden;
    box-shadow: 0 1px 3px rgba(0,0,0,.08); margin-bottom: 1.25rem;
  }
  th {
    background: #f8fafc; text-align: left;
    padding: .5rem .85rem; font-size: .75rem; color: #64748b; font-weight: 600;
    border-bottom: 1px solid #e2e8f0;
  }
  td { padding: .5rem .85rem; font-size: .85rem; border-top: 1px solid #f1f5f9; }
  tr.hl td { background: #f0fdf4; font-weight: 700; }
  td.ok   { color: #16a34a; font-weight: 600; }
  td.fail { color: #dc2626; font-weight: 600; }
  code { background: #f1f5f9; padding: .1rem .3rem; border-radius: 4px; font-size: .8rem; }
</style>
</head>
<body>

<h1>k6 Performance Report <span class="badge">${profile}</span></h1>
<div class="meta">Generated: ${now}</div>

<div class="cards">
  <div class="card">
    <div class="lbl">Total Requests</div>
    <div class="val">${num(reqs.count)}</div>
  </div>
  <div class="card">
    <div class="lbl">Throughput</div>
    <div class="val">${reqs.rate !== undefined ? reqs.rate.toFixed(1) : '-'}<small style="font-size:.75rem;font-weight:400"> req/s</small></div>
  </div>
  <div class="card">
    <div class="lbl">Iterations</div>
    <div class="val">${num(iters.count)}</div>
  </div>
  <div class="card">
    <div class="lbl">Max VUs</div>
    <div class="val">${num(vusMax.max)}</div>
  </div>
  <div class="card">
    <div class="lbl">Error Rate</div>
    <div class="val ${errorOk ? 'green' : 'red'}">${pct(failed.rate)}</div>
  </div>
  <div class="card">
    <div class="lbl">p95 Latency</div>
    <div class="val ${p95ok ? 'green' : 'red'}">${ms(dur['p(95)'])}</div>
  </div>
  <div class="card">
    <div class="lbl">p99 Latency</div>
    <div class="val">${ms(dur['p(99)'])}</div>
  </div>
  <div class="card">
    <div class="lbl">Avg Latency</div>
    <div class="val">${ms(dur.avg)}</div>
  </div>
</div>

<h2>Latency Distribution</h2>
<table>
  <thead><tr><th>Percentile</th><th>Value</th></tr></thead>
  <tbody>
    <tr><td>Average</td><td>${ms(dur.avg)}</td></tr>
    <tr><td>Minimum</td><td>${ms(dur.min)}</td></tr>
    <tr><td>Median (p50)</td><td>${ms(dur.med)}</td></tr>
    <tr><td>p90</td><td>${ms(dur['p(90)'])}</td></tr>
    <tr class="hl"><td>p95</td><td>${ms(dur['p(95)'])}</td></tr>
    <tr><td>p99</td><td>${ms(dur['p(99)'])}</td></tr>
    <tr><td>Maximum</td><td>${ms(dur.max)}</td></tr>
  </tbody>
</table>

<h2>Thresholds</h2>
<table>
  <thead><tr><th>Metric</th><th>Condition</th><th>Result</th></tr></thead>
  <tbody>${thresholdRows}</tbody>
</table>

<h2>Checks (${checks.length})</h2>
<table>
  <thead><tr><th>Check</th><th>Passed</th><th>Failed</th><th>Rate</th></tr></thead>
  <tbody>${checkRows}</tbody>
</table>

</body>
</html>`;
}
