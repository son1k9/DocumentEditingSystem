import React from 'react';
import { Link } from "react-router-dom";
import { Document } from '../models/Document';

interface DocumentCardProps {
    document: Document;
    onSettingsClick: () => void;
}

const DocumentCard: React.FC<DocumentCardProps> = ({ document, onSettingsClick }) => {
    return (
        <div style={styles.card}>
            <h2 style={styles.cardTitle}>{document.documentName}</h2>
            <div>
                <Link to={`/documents/${document.id}`}>
                    <button className="mt-2 px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600">
                        Редактировать
                    </button>
                </Link>
                <button className="ml-2 text-gray-700 hover:text-black transition duration-200"
                    onClick={onSettingsClick}>
                    ⚙️
                </button>
            </div>
        </div>
    );
};

const styles: { [key: string]: React.CSSProperties } = {
    card: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'space-between',
        alignItems: 'center',
        width: '300px',
        padding: '20px',
        border: '1px solid #ddd',
        borderRadius: '8px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        backgroundColor: 'white',
    },
    cardTitle: {
        margin: '0 0 10px 0',
        width: '100%',
    },
    button: {
        padding: '10px 20px',
        backgroundColor: '#3f51b5',
        color: 'white',
        border: 'none',
        borderRadius: '4px',
        cursor: 'pointer',
        fontSize: '1rem',
    },
};

export default DocumentCard;