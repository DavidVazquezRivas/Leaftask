export const BASE_URL = __ENV.BASE_URL || 'http://localhost:8080/api/v1';

export const THRESHOLDS = {
    http_req_duration: ['p(95)<500'],
    http_req_failed: ['rate<0.01'],
};

export const STATUS_ID = 'a1000000-0001-0000-0000-000000000001'; // "Por hacer"
export const TYPE_ID = 'b1000000-0001-0000-0000-000000000001';   // "Tarea"
