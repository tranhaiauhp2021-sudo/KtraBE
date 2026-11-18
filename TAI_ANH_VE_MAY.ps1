$ErrorActionPreference = 'Stop'
$folder = Join-Path $PSScriptRoot 'KtraBE\Content\products'
New-Item -ItemType Directory -Force -Path $folder | Out-Null
$photos = @(
    @{Id=1; Url='https://images.unsplash.com/photo-1724166595400-fdfcdb29685e?auto=format&fit=crop&w=960&q=85'},
    @{Id=2; Url='https://images.unsplash.com/photo-1695774165691-8a01a6045952?auto=format&fit=crop&w=960&q=85'},
    @{Id=3; Url='https://images.unsplash.com/photo-1520467795206-62e33627e6ce?auto=format&fit=crop&w=960&q=85'},
    @{Id=4; Url='https://images.unsplash.com/photo-1683920999075-c5e29a0bc306?auto=format&fit=crop&w=960&q=85'},
    @{Id=5; Url='https://images.unsplash.com/photo-1661246960287-c3942adab588?auto=format&fit=crop&w=960&q=85'},
    @{Id=6; Url='https://images.unsplash.com/photo-1688296524835-363a81b93921?auto=format&fit=crop&w=960&q=85'},
    @{Id=7; Url='https://images.unsplash.com/photo-1633354557121-98c12ac821bf?auto=format&fit=crop&w=960&q=85'},
    @{Id=8; Url='https://images.unsplash.com/photo-1711030238974-4e55cf76d497?auto=format&fit=crop&w=960&q=85'}
)
foreach ($photo in $photos) {
    $id = [int]$photo.Id
    $path = Join-Path $folder ("real-$id.jpg")
    try {
        Invoke-WebRequest -Uri ([string]$photo.Url) -OutFile $path -UseBasicParsing -TimeoutSec 40
        if ((Get-Item $path).Length -lt 10000) { throw 'Image file too small' }
        Write-Host ("Downloaded product $id") -ForegroundColor Green
    } catch {
        Write-Host ("Failed product $id - website will use online photograph") -ForegroundColor Yellow
        if (Test-Path $path) { Remove-Item $path -Force }
    }
}
Write-Host 'Completed. Rebuild (Ctrl+Shift+B), refresh browser (Ctrl+F5).'
Read-Host 'Press Enter to exit'
