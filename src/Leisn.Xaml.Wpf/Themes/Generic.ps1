$listFile = "Themes\Generic.txt"
$genericFile = "Themes\Generic.xaml"

$schema = @'
<!--This file is auto-generated, any changes of this file will lost after regenerated.--> 
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"></ResourceDictionary>
'@

Out-File -FilePath $genericFile -InputObject $schema -Encoding utf8

$genericXaml = [xml] (Get-Content $genericFile)
$doc = $genericXaml.DocumentElement

$readerSettings = New-Object -TypeName System.Xml.XmlReaderSettings
$readerSettings.IgnoreComments = {true}

$i = 0
$xamlList = Get-Content $listFile
foreach($line in $xamlList)
{
  if([String]::IsNullOrWhiteSpace($line) -or $line.StartsWith('#') -or $line.StartsWith("//")) {
    continue 
  }
  $i = $i+1
  Write-Host "Combining" $line
  #$xmldata = [xml](Get-Content $line)

  $xmldata = New-Object -TypeName System.Xml.XmlDocument
  $xmlReader = [System.Xml.XmlReader]::Create($line,$readerSettings)
  $xmldata.Load($xmlReader)
  $xmlReader.Dispose()

  $dataDoc = $xmldata.DocumentElement
  foreach($attr in $dataDoc.Attributes){
    $doc.SetAttribute($attr.Name,$attr.'#text')
  }
  foreach($node in $dataDoc.ChildNodes){
    $clone = $genericXaml.ImportNode($node,{true})
    $doc.AppendChild($clone)
  }
}

$settings = New-Object -TypeName System.Xml.XmlWriterSettings
$settings.Encoding=[System.Text.Encoding]::UTF8
$settings.OmitXmlDeclaration={true}
$settings.NamespaceHandling=[System.Xml.NamespaceHandling]::OmitDuplicates
#$settings.NewLineOnAttributes={true}
$settings.NewLineChars="`r`n"
$settings.NewLineHandling=[System.Xml.NewLineHandling]::Replace
$settings.IndentChars="  "
$settings.Indent={true}
$settings.CloseOutput={true}
$writer = [System.Xml.XmlWriter]::Create($genericFile,$settings)
$genericXaml.Save($writer)
$writer.Dispose()

Write-Host "Combining completed." 
Write-Host "File count = $i" 
