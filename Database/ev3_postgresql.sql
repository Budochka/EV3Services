-- PostgreSQL schema for EV3 Services Logger
-- Optimized for logging with proper indexes and data types
--
-- Usage:
--   1. Manual: Connect to ev3 database first, then run: psql -d ev3 -f ev3_postgresql.sql
--   2. Automated: Use create_database.ps1 (handles database creation and runs this file)
--
-- Note: This file can be run multiple times safely (uses IF NOT EXISTS where possible)

-- Events table with optimized schema
CREATE TABLE IF NOT EXISTS Events (
    ID SERIAL PRIMARY KEY,
    Time TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    Topic VARCHAR(255) NOT NULL,
    Data BYTEA,  -- Binary data (images, audio, etc.)
    DataSize INTEGER,  -- Size in bytes (for quick filtering without reading data)
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- Indexes for fast queries
CREATE INDEX IF NOT EXISTS idx_events_time ON Events(Time DESC);  -- DESC for recent-first queries
CREATE INDEX IF NOT EXISTS idx_events_topic ON Events(Topic);
CREATE INDEX IF NOT EXISTS idx_events_created ON Events(CreatedAt DESC);
CREATE INDEX IF NOT EXISTS idx_events_topic_time ON Events(Topic, Time DESC);  -- Composite for topic + time queries

-- Full-text search index on Topic (for searching routing keys)
-- Note: For full-text search on binary data content, consider extracting text metadata
CREATE INDEX IF NOT EXISTS idx_events_topic_gin ON Events USING GIN (to_tsvector('english', Topic));

-- Partitioning by month (optional, for very high volume)
-- Uncomment if you expect millions of events per month
-- CREATE TABLE Events_2024_01 PARTITION OF Events
--     FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');

-- Function to automatically set DataSize
CREATE OR REPLACE FUNCTION set_data_size()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.Data IS NOT NULL THEN
        NEW.DataSize := LENGTH(NEW.Data);
    ELSE
        NEW.DataSize := 0;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger to auto-calculate DataSize
DROP TRIGGER IF EXISTS trigger_set_data_size ON Events;
CREATE TRIGGER trigger_set_data_size
    BEFORE INSERT OR UPDATE ON Events
    FOR EACH ROW
    EXECUTE FUNCTION set_data_size();

-- View for text-based events (for easier querying)
-- Assumes UTF-8 text in Data column
CREATE OR REPLACE VIEW EventsText AS
SELECT 
    ID,
    Time,
    Topic,
    CASE 
        WHEN Data IS NOT NULL AND DataSize < 1048576 THEN  -- Only for data < 1MB
            CONVERT_FROM(Data, 'UTF8')
        ELSE NULL
    END AS DataText,
    DataSize,
    CreatedAt
FROM Events
WHERE Topic LIKE 'text.%' OR Topic LIKE 'sensors.%';  -- Adjust based on your text-based topics

-- Example queries:
-- 
-- Recent events (last hour):
-- SELECT * FROM Events WHERE Time > NOW() - INTERVAL '1 hour' ORDER BY Time DESC;
--
-- Events by topic:
-- SELECT * FROM Events WHERE Topic LIKE 'images.%' ORDER BY Time DESC LIMIT 100;
--
-- Full-text search on topics:
-- SELECT * FROM Events WHERE to_tsvector('english', Topic) @@ to_tsquery('english', 'face & detection');
--
-- Large binary data (images/audio):
-- SELECT ID, Time, Topic, DataSize FROM Events WHERE DataSize > 100000 ORDER BY Time DESC;
--
-- Cleanup old events (older than 30 days):
-- DELETE FROM Events WHERE Time < NOW() - INTERVAL '30 days';

