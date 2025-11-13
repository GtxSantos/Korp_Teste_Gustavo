Write-Output "=== Simulação de falha: impressão com servico-estoque parado ==="

# 1) Cria um produto com saldo 1
$body = @{ Codigo='LUPA'; Descricao='Produto Lupa'; Saldo=1 } | ConvertTo-Json
$r = Invoke-RestMethod -Uri 'http://localhost:5000/api/v1/estoque/produtos' -Method Post -ContentType 'application/json' -Body $body -ErrorAction Stop
$prodId = $r.Id
Write-Output "Produto criado: $prodId (saldo 1)"

# 2) Cria uma nota que usa 1 unidade (deve funcionar)
$nota = @{ Itens = @(@{ ProdutoId = $prodId; Quantidade = 1 }) } | ConvertTo-Json
$nr = Invoke-RestMethod -Uri 'http://localhost:5126/api/v1/faturamento/notas' -Method Post -ContentType 'application/json' -Body $nota -ErrorAction Stop
Write-Output "Nota criada: $($nr.Id)"

# 3) Para o servico-estoque (simula falha)
$procs = Get-Process -Name servico-estoque -ErrorAction SilentlyContinue
if ($procs) {
    foreach ($p in $procs) { Write-Output "Matando processo servico-estoque PID $($p.Id)"; Stop-Process -Id $p.Id -Force }
} else {
    Write-Output "Nenhum processo servico-estoque encontrado — continuando (a simulação pode não reproduzir falha).";
}

Start-Sleep -Seconds 1

# 4) Tenta imprimir — deve retornar erro 503
try {
    $im = Invoke-RestMethod -Uri ("http://localhost:5126/api/v1/faturamento/notas/{0}/imprimir" -f $nr.Id) -Method Post -ErrorAction Stop
    Write-Output "Impressão (unexpected): $($im | ConvertTo-Json)"
} catch {
    Write-Output "Erro esperado ao imprimir: $($_.Exception.Response.StatusCode) - $($_.Exception.Message)"
    try { $resp = $_.Exception.Response | Select-Object -ExpandProperty Content; Write-Output "Conteúdo: $resp" } catch {}
}

Write-Output "=== Simulação concluída ==="
