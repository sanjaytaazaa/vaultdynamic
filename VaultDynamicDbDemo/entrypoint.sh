# Wait for the secrets file to be created AND have content
while [ ! -s /vault/secrets/db-secrets.json ]; do
  sleep 2
done

echo "Secrets are ready. Starting the ASP.NET application..."

# Use the environment variable for the DLL path
exec dotnet VaultDynamicDbDemo.dll
