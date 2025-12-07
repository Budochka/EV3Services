# RabbitMQ Firehose Tracer - Enable Instructions

## Current Status

✅ **Trace Exchange Exists**: `amq.rabbitmq.trace` exchange is present on `storage2:5672`
- Exchange Type: `topic`
- Durable: `True`
- Internal: `True`

⚠️ **Tracing Status**: Unknown - The exchange exists, but tracing may not be actively routing messages to it.

## How to Enable Firehose Tracer

The `amq.rabbitmq.trace` exchange exists by default in RabbitMQ, but **tracing must be enabled** to actually route messages to it. Unfortunately, enabling tracing typically requires the `rabbitmqctl` command on the server.

### Option 1: Enable via SSH/Remote Access to Server

If you have SSH access to the `storage2` server:

```bash
# SSH to storage2
ssh user@storage2

# Enable tracing for default vhost (/)
rabbitmqctl trace_on

# Or enable for specific vhost
rabbitmqctl trace_on -p /vhost_name

# Check if tracing is enabled
rabbitmqctl trace_status
```

### Option 2: Enable via Management UI

1. **Access Management UI**: Navigate to `http://storage2:15672/`
2. **Login**: Use your RabbitMQ credentials (default: guest/guest)
3. **Check Admin Tab**: Look for "Tracing" or "Firehose" options
4. **Enable**: If available, enable tracing from the UI

**Note**: The Management UI may not have a direct "enable firehose" option. The `rabbitmq_tracing` plugin (different from firehose) can be enabled via UI, but the built-in firehose tracer typically requires `rabbitmqctl`.

### Option 3: Use RabbitMQ Management HTTP API (If Available)

Try these API endpoints (may not be available in all RabbitMQ versions):

```powershell
# Check if trace is enabled
$cred = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes("guest:guest"))
Invoke-WebRequest -Uri "http://storage2:15672/api/trace/%2F" `
    -Headers @{Authorization="Basic $cred"} -Method GET

# Enable trace (if endpoint exists)
Invoke-WebRequest -Uri "http://storage2:15672/api/trace/%2F" `
    -Headers @{Authorization="Basic $cred"} -Method PUT
```

### Option 4: Alternative - Use Exchange-to-Exchange Binding

If you cannot enable firehose tracer, use **Exchange-to-Exchange Binding** instead:

1. Create a logging exchange: `EV3-Log`
2. Bind it to the main `EV3` exchange
3. Logger consumes from `EV3-Log` instead

This achieves the same result (non-consuming logging) without requiring firehose tracer.

## Verify Tracing is Active

### Method 1: Check Exchange Activity

Monitor the `amq.rabbitmq.trace` exchange in Management UI:
- Go to Exchanges → `amq.rabbitmq.trace`
- Check "Message rates" - if tracing is active, you should see messages flowing

### Method 2: Test with Logger

Update Logger to consume from `amq.rabbitmq.trace` and check if messages are received:
- If messages appear → Tracing is enabled ✅
- If no messages → Tracing is disabled ❌

### Method 3: Check Server Logs

On the RabbitMQ server, check logs for trace-related messages:
```bash
tail -f /var/log/rabbitmq/rabbitmq.log | grep -i trace
```

## Performance Impact

⚠️ **Warning**: Enabling firehose tracer **doubles message volume**:
- Every published message → creates a trace message
- Every delivered message → creates a trace message
- High-traffic systems may experience performance impact

**Recommendation**: 
- Enable only when needed for logging
- Monitor server performance
- Consider filtering by routing key pattern if volume is too high

## Disable Tracing

When logging is no longer needed:

```bash
# Via rabbitmqctl (on server)
rabbitmqctl trace_off

# Or via Management API (if available)
Invoke-WebRequest -Uri "http://storage2:15672/api/trace/%2F" `
    -Headers @{Authorization="Basic $cred"} -Method DELETE
```

## Next Steps

1. **Enable tracing** using one of the methods above
2. **Verify** tracing is active (check exchange activity)
3. **Update Logger** to consume from `amq.rabbitmq.trace` exchange
4. **Parse trace messages** (format: `publish.exchange.routing_key` or `deliver.queue.routing_key`)
5. **Monitor performance** and disable if needed

## Alternative: Exchange-to-Exchange Binding

If firehose tracer cannot be enabled, implement Exchange-to-Exchange Binding (see `LOGGING_APPROACHES_PROPOSAL.md` Option 2) - this doesn't require any server-side configuration changes.

