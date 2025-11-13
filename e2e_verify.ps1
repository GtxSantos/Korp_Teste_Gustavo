Start-Sleep -Seconds 4
Write-Output "Criando produto..."
$body = @{ Codigo='P-TEST'; Descricao='Produto Teste'; Saldo=10 } | ConvertTo-Json
try {
    $r = Invoke-RestMethod -Uri 'http://localhost:5000/api/v1/estoque/produtos' -Method Post -ContentType 'application/json' -Body $body -ErrorAction Stop
    Write-Output "Produto criado: $($r.Id)"
} catch {
    Write-Output "Falha ao criar produto: $_"
    exit 1
}
$prodId = $r.Id
Start-Sleep -Seconds 1
Write-Output "Criando nota..."
$nota = @{ Itens = @(@{ ProdutoId = $prodId; Quantidade = 2 }) } | ConvertTo-Json
try {
    $nr = Invoke-RestMethod -Uri 'http://localhost:5126/api/v1/faturamento/notas' -Method Post -ContentType 'application/json' -Body $nota -ErrorAction Stop
    Write-Output "Nota criada: $($nr.Id)"
} catch {
    Write-Output "Falha ao criar nota: $_"
    exit 1
}
Start-Sleep -Seconds 1
Write-Output "Imprimindo nota..."
try {
    $imp = Invoke-RestMethod -Uri ("http://localhost:5126/api/v1/faturamento/notas/{0}/imprimir" -f $nr.Id) -Method Post -ErrorAction Stop
    Write-Output ("Impressão resultado: " + ($imp.Status))
} catch {
    Write-Output "Falha ao imprimir nota: $_"
    exit 1
}
Start-Sleep -Seconds 1
Write-Output "Verificando produto..."
try {
    $prodAfter = Invoke-RestMethod -Uri ("http://localhost:5000/api/v1/estoque/produtos/{0}" -f $prodId) -Method Get -ErrorAction Stop
    Write-Output ("Saldo final: " + $prodAfter.Saldo)
} catch {
    Write-Output "Falha ao buscar produto: $_"
    exit 1
}
