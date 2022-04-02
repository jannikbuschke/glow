# rimraf core-web/node_modules
# rimraf core-web/package-lock.json

$paths = @("core-web", "glow.azure-web", "glow.beta-web", ".")
# $paths = @("core-web")

foreach ($path in $paths){
  $node_modules = "$path/node_modules"
  echo "rimraf $node_modules"
  rimraf $node_modules
  $package_lock = "$path/package-lock.json"
  echo "rimraf $package_lock"
  rimraf $package_lock
}

echo "npm i"
npm i
echo "npm run boostrap"
npm run bootstrap