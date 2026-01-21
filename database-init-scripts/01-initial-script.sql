-- ============================================================================
-- Database Creation Script for EventService
-- ============================================================================

-- ============================================================================
-- Schema Creation
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS events;

-- Set search path
SET search_path TO events, public;

-- ============================================================================
-- Enumeration Tables
-- ============================================================================

-- EventStatus enumeration
CREATE TABLE IF NOT EXISTS events.event_statuses (
    id INTEGER PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    display_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO events.event_statuses (id, name, display_name) VALUES
(1, 'Draft', 'Черновик'),
(2, 'Published', 'Опубликовано'),
(3, 'Cancelled', 'Отменено'),
(4, 'Completed', 'Завершено')
ON CONFLICT (id) DO NOTHING;

-- ParticipantStatus enumeration
CREATE TABLE IF NOT EXISTS events.participant_statuses (
    id INTEGER PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    display_name VARCHAR(100) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO events.participant_statuses (id, name, display_name) VALUES
(1, 'Registered', 'Зарегистрирован'),
(2, 'Attended', 'Присутствовал'),
(3, 'Cancelled', 'Отменил участие')
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- Entity Tables
-- ============================================================================

-- EventType table
CREATE TABLE IF NOT EXISTS events.event_types (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(100) NOT NULL,
    description TEXT NOT NULL,
    icon VARCHAR(255) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Organizer table
CREATE TABLE IF NOT EXISTS events.organizers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(50),
    company_name VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_email CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- Events table (Aggregate Root)
CREATE TABLE IF NOT EXISTS events.events (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    title VARCHAR(500) NOT NULL,
    description TEXT NOT NULL,
    event_type_id UUID NOT NULL,
    organizer_id UUID NOT NULL,
    
    -- Location value object (embedded)
    location_address VARCHAR(500) NOT NULL,
    location_city VARCHAR(100) NOT NULL,
    location_country VARCHAR(100) NOT NULL,
    location_latitude DECIMAL(10, 8),
    location_longitude DECIMAL(11, 8),
    
    start_date TIMESTAMP WITH TIME ZONE NOT NULL,
    end_date TIMESTAMP WITH TIME ZONE NOT NULL,
    max_participants INTEGER NOT NULL,
    status_id INTEGER NOT NULL DEFAULT 1,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE,
    
    CONSTRAINT fk_event_type FOREIGN KEY (event_type_id) 
        REFERENCES events.event_types(id) ON DELETE RESTRICT,
    CONSTRAINT fk_organizer FOREIGN KEY (organizer_id) 
        REFERENCES events.organizers(id) ON DELETE RESTRICT,
    CONSTRAINT fk_status FOREIGN KEY (status_id) 
        REFERENCES events.event_statuses(id) ON DELETE RESTRICT,
    CONSTRAINT valid_dates CHECK (end_date > start_date),
    CONSTRAINT valid_max_participants CHECK (max_participants > 0),
    CONSTRAINT valid_latitude CHECK (location_latitude IS NULL OR (location_latitude >= -90 AND location_latitude <= 90)),
    CONSTRAINT valid_longitude CHECK (location_longitude IS NULL OR (location_longitude >= -180 AND location_longitude <= 180))
);

-- Participants table (Entity, part of Event aggregate)
CREATE TABLE IF NOT EXISTS events.participants (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    event_id UUID NOT NULL,
    user_id VARCHAR(255) NOT NULL,
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    registered_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    status_id INTEGER NOT NULL DEFAULT 1,
    
    CONSTRAINT fk_event FOREIGN KEY (event_id) 
        REFERENCES events.events(id) ON DELETE CASCADE,
    CONSTRAINT fk_participant_status FOREIGN KEY (status_id) 
        REFERENCES events.participant_statuses(id) ON DELETE RESTRICT,
    CONSTRAINT valid_participant_email CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT unique_participant_per_event UNIQUE (event_id, user_id)
);

-- Reviews table (Entity, part of Event aggregate)
CREATE TABLE IF NOT EXISTS events.reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    event_id UUID NOT NULL,
    user_id VARCHAR(255) NOT NULL,
    user_name VARCHAR(255) NOT NULL,
    rating INTEGER NOT NULL,
    comment TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_review_event FOREIGN KEY (event_id) 
        REFERENCES events.events(id) ON DELETE CASCADE,
    CONSTRAINT valid_rating CHECK (rating >= 1 AND rating <= 5),
    CONSTRAINT unique_review_per_user UNIQUE (event_id, user_id)
);

-- ============================================================================
-- Indexes for Performance Optimization
-- ============================================================================

-- Events table indexes
CREATE INDEX IF NOT EXISTS idx_events_status ON events.events(status_id);
CREATE INDEX IF NOT EXISTS idx_events_event_type ON events.events(event_type_id);
CREATE INDEX IF NOT EXISTS idx_events_organizer ON events.events(organizer_id);
CREATE INDEX IF NOT EXISTS idx_events_start_date ON events.events(start_date);
CREATE INDEX IF NOT EXISTS idx_events_end_date ON events.events(end_date);
CREATE INDEX IF NOT EXISTS idx_events_location_city ON events.events(location_city);
CREATE INDEX IF NOT EXISTS idx_events_location_country ON events.events(location_country);

-- Participants table indexes
CREATE INDEX IF NOT EXISTS idx_participants_event ON events.participants(event_id);
CREATE INDEX IF NOT EXISTS idx_participants_user ON events.participants(user_id);
CREATE INDEX IF NOT EXISTS idx_participants_status ON events.participants(status_id);
CREATE INDEX IF NOT EXISTS idx_participants_email ON events.participants(email);

-- Reviews table indexes
CREATE INDEX IF NOT EXISTS idx_reviews_event ON events.reviews(event_id);
CREATE INDEX IF NOT EXISTS idx_reviews_user ON events.reviews(user_id);
CREATE INDEX IF NOT EXISTS idx_reviews_rating ON events.reviews(rating);

-- Organizers table indexes
CREATE INDEX IF NOT EXISTS idx_organizers_email ON events.organizers(email);

-- Event types table indexes
CREATE INDEX IF NOT EXISTS idx_event_types_name ON events.event_types(name);

-- ============================================================================
-- Triggers for automatic updated_at timestamp
-- ============================================================================

-- Function to update updated_at column
CREATE OR REPLACE FUNCTION events.update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger for events table
DROP TRIGGER IF EXISTS update_events_updated_at ON events.events;
CREATE TRIGGER update_events_updated_at 
    BEFORE UPDATE ON events.events
    FOR EACH ROW 
    EXECUTE FUNCTION events.update_updated_at_column();

-- Trigger for event_types table
DROP TRIGGER IF EXISTS update_event_types_updated_at ON events.event_types;
CREATE TRIGGER update_event_types_updated_at 
    BEFORE UPDATE ON events.event_types
    FOR EACH ROW 
    EXECUTE FUNCTION events.update_updated_at_column();

-- Trigger for organizers table
DROP TRIGGER IF EXISTS update_organizers_updated_at ON events.organizers;
CREATE TRIGGER update_organizers_updated_at 
    BEFORE UPDATE ON events.organizers
    FOR EACH ROW 
    EXECUTE FUNCTION events.update_updated_at_column();

-- ============================================================================
-- Permissions
-- ============================================================================

-- Grant permissions (adjust based on your application user)
GRANT USAGE ON SCHEMA events TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA events TO postgres;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA events TO postgres;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA events TO postgres;

-- ============================================================================
-- Database Statistics and Verification
-- ============================================================================

-- View table structure summary
DO $$
DECLARE
    v_table_count INTEGER;
BEGIN
    SELECT COUNT(*) INTO v_table_count 
    FROM information_schema.tables 
    WHERE table_schema = 'events' AND table_type = 'BASE TABLE';
    
    RAISE NOTICE '============================================================================';
    RAISE NOTICE 'Database initialization completed successfully!';
    RAISE NOTICE '============================================================================';
    RAISE NOTICE 'Schema: events';
    RAISE NOTICE 'Tables created: %', v_table_count;
    RAISE NOTICE '----------------------------------------------------------------------------';
    RAISE NOTICE 'Tables:';
    RAISE NOTICE '  - event_statuses (enumeration)';
    RAISE NOTICE '  - participant_statuses (enumeration)';
    RAISE NOTICE '  - event_types (entity)';
    RAISE NOTICE '  - organizers (entity)';
    RAISE NOTICE '  - events (aggregate root)';
    RAISE NOTICE '  - participants (entity - part of events aggregate)';
    RAISE NOTICE '  - reviews (entity - part of events aggregate)';
    RAISE NOTICE '============================================================================';
END $$;